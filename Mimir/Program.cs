using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Mimir
{
    public class Program
    {
        #region Global settings
        internal static SqlDbType SqlType = SqlDbType.MySql;
        internal static string SqlIP = "127.0.0.1";
        internal static int SqlPort = 3306;
        internal static string SqlUsername = "root";
        internal static string SqlPassword = "123";
        internal static string SqlDatabaseName = "mimir";

        internal readonly static string Version = "1.0.0";

        internal static string ServerName = "Mimir Server";
        internal static RSACryptoServiceProvider PrivateKeyProvider = new RSACryptoServiceProvider();
        internal static string PublicKey = "";
        internal static string ServerDomain = "";
        internal static int SecurityLoginTryTimes = 5;
        internal static bool IsEnableMultiProfiles = true;
        internal static int MaxTokensPerProfile = 10;
        internal static int TokensExpireDaysLimit = 15;
        internal static long SessionsExpireSeconds = 30;
        internal static string[] SkinDomains = null;
        internal static int MaxProfileCountPerQuery = 2;
        internal static bool IsEnableLandingPage = true;
        internal static bool IsEnableSmtp = false;
        internal static string SmtpDomain = string.Empty;
        internal static int SmtpPort = 465;
        internal static string SmtpEmail = string.Empty;
        internal static string SmtpName = string.Empty;
        internal static string SmtpPassword = string.Empty;
        internal static bool SmtpIsSsl = false;
        #endregion

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    var config = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));

                    // Load database configs.
                    Enum.TryParse(config["Database:Type"], out SqlType);
                    SqlIP = config["Database:IP"];
                    int.TryParse(config["Database:Port"], out SqlPort);
                    SqlUsername = config["Database:User"];
                    SqlPassword = config["Database:Password"];
                    SqlDatabaseName = config["Database:Name"];

                    if (bool.Parse(config["Listen:Http:Enable"]))
                    {
                        options.Listen(IPAddress.Any, int.Parse(config["Listen:Http:Port"]), opt =>
                        {
                            opt.UseConnectionLogging();
                        });
                    }

                    if (bool.Parse(config["Listen:Https:Enable"]))
                    {
                        options.Listen(IPAddress.Any, int.Parse(config["Listen:Https:Port"]), opt =>
                        {
                            opt.UseHttps(config["Listen:Https:Cert"], config["Listen:Https:Password"]);
                            opt.UseConnectionLogging();
                        });
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                }).UseNLog();

        internal enum SqlDbType
        {
            MySql, 
            Sqlite
        }
    }
}
