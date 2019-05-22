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

            services.AddSession();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Index/Error");
            }

            // Load database configs.
            log.Info("Loading database configs.");
            Enum.TryParse(Configuration["Database:Type"], out Program.SqlType);
            Program.SqlIP = Configuration["Database:IP"];
            int.TryParse(Configuration["Database:Port"], out Program.SqlPort);
            Program.SqlUsername = Configuration["Database:User"];
            Program.SqlPassword = Configuration["Database:Password"];
            Program.SqlDatabaseName = Configuration["Database:Name"];
            log.Info("Database configs loaded.");

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
            }
            catch (Exception)
            {
                log.Fatal("Bad database.");
                throw;
            }
            log.Info("Logic configs loaded.");

            app.UseSession();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Add ALI.
            app.Use(next =>
            {
                return async context =>
                {
                    context.Response.OnStarting(() =>
                    {
                        if (Program.IsHttps)
                        {
                            context.Response.Headers.Add("X-Authlib-Injector-API-Location", "https://" + Program.ServerDomain + "/api/");
                        }
                        else
                        {
                            context.Response.Headers.Add("X-Authlib-Injector-API-Location", "http://" + Program.ServerDomain + "/api/");
                        }
                        return Task.CompletedTask;
                    });
                    await next(context);
                };
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "get_profile",
                    template: "api/sessionserver/session/minecraft/profile/{uuid}", 
                    defaults: new { controller = "SessionServer", action = "Profile"});
                routes.MapRoute(
                    name: "session_join",
                    template: "api/sessionserver/session/minecraft/join", 
                    defaults: new { controller = "SessionServer", action = "Join"});
                routes.MapRoute(
                    name: "session_has_joined",
                    template: "api/sessionserver/session/minecraft/hasJoined",
                    defaults: new { controller = "SessionServer", action = "HasJoined" });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "yggdrasil",
                    template: "api/{controller=Api}/{action=Index}");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Index}/{action=Index}/{id?}");
            });
        }
    }
}
