using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mimir.Controllers;
using Mimir.Models;
using Mimir.Util;
using Newtonsoft.Json;
using NLog;

namespace Mimir
{
    public class Startup
    {
        private readonly ILogger log;

        public Startup(IConfiguration configuration)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Configuration = configuration;
            log = LogManager.GetLogger("Mimir");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MimirContext>(option =>
            {
                switch (Program.SqlType)
                {
                    case Program.SqlDbType.MySql:
                        option.UseMySQL($"Server={Program.SqlIP};Port={Program.SqlPort};Uid={Program.SqlUsername};Password={Program.SqlPassword};DataBase={Program.SqlDatabaseName};");
                        break;
                    case Program.SqlDbType.Sqlite:
                        option.UseSqlite($"Data Source=/{Program.SqlDatabaseName}.db;Version=3;Password={Program.SqlPassword};");
                        break;
                }
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.Name = ".Mimir.Session";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MimirContext db)
        {
            if (env.IsDevelopment())
            {
                log.Info("LittleC is a potato!");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            // Load logic configs.
            log.Info("Loading logic configs.");
            try
            {
                Program.ServerName = (from o in db.Options where o.Option == "ServerName" select o.Value).First();
                if ((from o in db.Options where o.Option == "PrivateKeyXml" select o.Value).First() == string.Empty)
                {
                    SignatureWorker.GenKey(db);
                }
                RSACryptoServiceProviderExtensions.FromXmlString(Program.PrivateKeyProvider, 
                    (from o in db.Options where o.Option == "PrivateKeyXml" select o.Value).First());
                Program.PublicKey = (from o in db.Options where o.Option == "PublicKey" select o.Value).First();
                Program.ServerDomain = (from o in db.Options where o.Option == "ServerDomain" select o.Value).First();
                int.TryParse((from o in db.Options where o.Option == "SecurityLoginTryTimes" select o.Value).First(), 
                    out Program.SecurityLoginTryTimes);
                bool.TryParse((from o in db.Options where o.Option == "IsEnableMultiProfiles" select o.Value).First(),
                    out Program.IsEnableMultiProfiles);
                int.TryParse((from o in db.Options where o.Option == "MaxTokensPerProfile" select o.Value).First(),
                    out Program.MaxTokensPerProfile);
                int.TryParse((from o in db.Options where o.Option == "TokensExpireDaysLimit" select o.Value).First(),
                    out Program.TokensExpireDaysLimit);
                long.TryParse((from o in db.Options where o.Option == "SessionsExpireSeconds" select o.Value).First(),
                    out Program.SessionsExpireSeconds);
                bool.TryParse((from o in db.Options where o.Option == "IsHttps" select o.Value).First(),
                    out Program.IsHttps);
                Program.SkinDomains = (from o in db.Options where o.Option == "SkinDomains" select o.Value).First().Split(",");
                int.TryParse((from o in db.Options where o.Option == "MaxProfileCountPerQuery" select o.Value).First(),
                    out Program.MaxProfileCountPerQuery);
            }
            catch (Exception)
            {
                log.Fatal("Bad database.");
                throw;
            }
            log.Info("Logic configs loaded.");

            app.UseSession();

            //app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "image/png"
            });

            app.Use(next =>
            {
                return async context =>
                {
                    context.Response.OnStarting(() =>
                    {
                        // Add ALI.
                        if (Program.IsHttps)
                        {
                            context.Response.Headers.Add("X-Authlib-Injector-API-Location", "https://" + Program.ServerDomain + "/api/");
                        }
                        else
                        {
                            context.Response.Headers.Add("X-Authlib-Injector-API-Location", "http://" + Program.ServerDomain + "/api/");
                        }

                        context.Response.Headers["Server"] = "Mimir";
                        context.Response.Headers.Add("Author", "Romonov");
                        return Task.CompletedTask;
                    });
                    await next(context);
                };
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "yggdrasil_sessionserver_get_profile",
                    template: "api/sessionserver/session/minecraft/profile/{uuid}",
                    defaults: new { controller = "SessionServer", action = "Profile" });
                routes.MapRoute(
                    name: "yggdrasil_sessionserver_join",
                    template: "api/sessionserver/session/minecraft/join",
                    defaults: new { controller = "SessionServer", action = "Join" });
                routes.MapRoute(
                    name: "yggdrasil_sessionserver_has_joined",
                    template: "api/sessionserver/session/minecraft/hasJoined",
                    defaults: new { controller = "SessionServer", action = "HasJoined" });
                routes.MapRoute(
                    name: "yggdrasil_api_profiles_query",
                    template: "api/api/profiles/minecraft", 
                    defaults: new { controller = "Api", action = "Profiles" });
                routes.MapRoute(
                    name: "yggdrasil_authserver",
                    template: "api/authserver/{action}",
                    defaults: new { controller = "AuthServer" });
                routes.MapRoute(
                    name: "yggdrasil_index",
                    template: "api", 
                    defaults: new { controller = "Api", action = "Index"});
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Index}/{action=Index}/{id?}");
            });
        }
    }
}
