using Mimir.Common;
using Mimir.SQL;
using RUL;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Mimir
{
    class Program
    {
        #region 定义变量
        public const string Name = "Mimir";
        public const string Version = "0.5.3";

        public static string Path = Directory.GetCurrentDirectory();

        public static string SQLIP = "localhost";
        public static string SQLUsername = "root";
        public static string SQLPassword = "123456";
        public const string SQLDatabase = "mimir";

        public static string SslCertName = "ssl.pfx";
        public static string SslCertPassword = "123";

        public static string SkinPublicKey = "";

        public static string ServerName = "Mimir Server";

        public static string[] SkinDomains;

        public static int Port = 45672;
        public static int MaxConnection = 233;
        public static int SkinDomainsCount = 1;

        public static bool IsRunning = false;
        public static bool IsSslEnabled = false;
        public static bool IsCustomCert = true;

        SocketWorker SocketWorker = new SocketWorker();

        public static X509Certificate2 ServerCertificate = new X509Certificate2();

        public static ConfigWorker.SQLType SQLType = ConfigWorker.SQLType.MySql;
        #endregion

        static void Main(string[] args)
        {
            Logger.Info($"Mimir version: {Version}, made by: Romonov! ");
            Logger.Info("Starting...");

            if (!new Program().Init())
            {
                Logger.Info("Init Failed.");
                Console.Read();
                return;
            }

            Logger.Info("Welcome!");

            // 主循环 
            while (true)
            {
                string input = Console.ReadLine();
                Logger.WriteToFile(input);
                switch (input)
                {
                    case "stop":
                        SqlProxy.Close();
                        ConfigWorker.Save(Directory.GetCurrentDirectory() + @"\config.ini");
                        Environment.Exit(0);
                        break;
                    default:
                        Logger.Error("No such command.");
                        continue;
                }
            }
        }

        // 初始化
        bool Init()
        {
            // 加载配置文件
            Logger.Info("Loading configs...");
            string ConfigPath = Directory.GetCurrentDirectory() + @"\config.ini";

            if (!File.Exists(ConfigPath))
            {
                ConfigWorker.Init(ConfigPath);
            }
            else
            {
                ConfigWorker.Read(ConfigPath);
            }
  
            Logger.Info("Configs loaded!");

            // 加载签名秘钥
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\PrivateKey.xml"))
            {
                Logger.Warn("Private key file is missing, and it will be generated now.");
                if (!RSAWorker.GenKey())
                {
                    return false;
                }
            }

            if (!RSAWorker.LoadKey())
            {
                return false;
            }

            SkinPublicKey = RSAWorker.RSAPublicKeyConverter(File.ReadAllText(Directory.GetCurrentDirectory() + @"\PublicKey.xml"));

            // 加载SSL证书
            try
            {
                if (IsSslEnabled)
                {
                    CertWorker.Load();
                }
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
            }

            /*
            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, "Mimir", false);
            ServerCertificate = certs[0];
            store.Close();
            */

            // 打开SQL和Socket链接
            try
            {
                SqlProxy.Open();

                SocketWorker.Init(Port, MaxConnection);
                SocketWorker.Start();
            }
            catch(Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }            

            IsRunning = true;

            return true;
        }
    }
}
