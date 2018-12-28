using Mimir.Common;
using Mimir.Common.SQL;
using RUL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir
{
    class Program
    {
        #region 定义变量
        public const string Name = "Mimir";
        public const string Version = "0.6.5";

        public static string Path = Directory.GetCurrentDirectory();

        public static string SQLIP = "localhost";
        public static string SQLUsername = "root";
        public static string SQLPassword = "123456";
        public const string SQLDatabase = "mimir";
        
        public static string SkinPublicKey = "";

        public static string ServerName = "Mimir Server";

        public static IPAddress FrontEndAddress = IPAddress.Loopback;

        public static string[] SkinDomains;

        public static int Port = 45672;
        public static int MaxConnection = 233;
        public static int SkinDomainsCount = 1;

        public static bool IsRunning = false;
        public static bool IsDebug = false;

        static SocketWorker SocketListener = new SocketWorker();
        
        public static ConfigWorker.SQLType SQLType = ConfigWorker.SQLType.MySql;

        private static Logger log = new Logger("Main");
        #endregion

        static void Main(string[] args)
        {
            log.Info($"Mimir version: {Version}, made by: Romonov! ");
            log.Info("Starting...");

            if (!Init())
            {
                log.Error("Init failed!\a");
                Console.Read();
                Environment.Exit(1);
            }

            log.Info("Welcome!!");

            // 主循环 
            while (true)
            {
                string input = Console.ReadLine();
                log.WriteToFile(input);
                switch (input)
                {
                    case "stop":
                        SqlProxy.Close();
                        ConfigWorker.Save(Directory.GetCurrentDirectory() + @"\config.ini");
                        Console.Read();
                        Environment.Exit(0);
                        break;
                    default:
                        log.Error("No such command.");
                        continue;
                }
            }
        }

        // 初始化
        static bool Init()
        {
            try
            {
                // 加载配置文件
                log.Info("Loading configs...");
                string configPath = Directory.GetCurrentDirectory() + @"\config.ini";

                if (!File.Exists(configPath))
                {
                    ConfigWorker.Init(configPath);
                }
                else
                {
                    ConfigWorker.Read(configPath);
                }

                string texturesPath = Directory.GetCurrentDirectory() + @"\Textures";
                if (!Directory.Exists(texturesPath))
                {
                    Directory.CreateDirectory(texturesPath);
                }

                log.Info("Configs loaded!");

                // 加载签名秘钥
                if (!File.Exists(Directory.GetCurrentDirectory() + @"\PrivateKey.xml"))
                {
                    log.Warn("Private key file is missing, and it will be generated now.");
                    RSAWorker.GenKey();
                }

                RSAWorker.LoadKey();

                SkinPublicKey = RSAWorker.RSAPublicKeyConverter(RSAWorker.PublicKey.ToXmlString(false));

                // 打开SQL和Socket链接
                SqlProxy.Open();

                SocketListener.Init(Port, MaxConnection);
                SocketListener.Start();

                IsRunning = true;

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }

            return true;
        }
    }
}
