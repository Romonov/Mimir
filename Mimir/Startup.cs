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
                Program.ServerName = (from options in db.Options where options.Option == "ServerName" select options.Value).First();
                RSACryptoServiceProviderExtensions.FromXmlString(Program.PrivateKeyProvider, 
                    (from options in db.Options where options.Option == "PrivateKeyXml" select options.Value).First());
                Program.PublicKey = (from options in db.Options where options.Option == "PublicKey" select options.Value).First();
                Program.ServerDomain = (from options in db.Options where options.Option == "ServerDomain" select options.Value).First();
                int.TryParse((from options in db.Options where options.Option == "SecurityLoginTryTimes" select options.Value).First(), 
                    out Program.SecurityLoginTryTimes);
                bool.TryParse((from options in db.Options where options.Option == "IsEnableMultiProfiles" select options.Value).First(),
                    out Program.IsEnableMultiProfiles);
                int.TryParse((from options in db.Options where options.Option == "MaxTokensPerUser" select options.Value).First(),
                    out Program.MaxTokensPerUser);
                int.TryParse((from options in db.Options where options.Option == "TokensExpireDaysLimit" select options.Value).First(),
                    out Program.TokensExpireDaysLimit);
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
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "session_join",
                    template: "api/sessionserver/session/minecraft/join/{uuid?}", 
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
