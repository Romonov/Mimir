using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal static string SqlPassword = "123456";
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
        #endregion

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
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
