﻿using Mimir.Common;
using Mimir.SQL;
using RUL;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mimir
{
    class Program
    {
        #region 定义变量
        public const string Name = "Mimir";
        public const string Version = "0.3.1";

        public static string Path = Directory.GetCurrentDirectory();

        public static string SQLIP = "localhost";
        public static string SQLUsername = "root";
        public static string SQLPassword = "123456";
        public const string SQLDatabase = "mimir";

        public static string SslCAName = "ca_bundle.crt";
        public static string SslCAContent = "";
        public static string SslCertName = "certificate.crt";
        public static string SslCertContent = "";
        public static string SslKeyName = "private.key";
        public static string SslKeyContent = "";

        public static string SkinPublicKey = "";

        public static string ServerName = "Mimir Server";

        public static string[] SkinDomains;

        public static int Port = 45672;
        public static int MaxConnection = 233;
        public static int SkinDomainsCount = 1;

        public static bool IsRunning = false;
        public static bool IsSslEnabled = false;
        public static bool IsSslUseCertChain = true;

        SocketWorker SocketWorker = new SocketWorker();

        public static X509Certificate serverCertificate = new X509Certificate();

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

            X509Store store = new X509Store(StoreName.Root);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, "Mimir", false);
            serverCertificate = certs[0];

            Logger.Info("Welcome!");

            // 主循环 
            while (true)
            {
                string input = Console.ReadLine();
                Logger.WriteToFile(input);
                switch (input)
                {
                    case "stop":
                        MySqlWorker.Close();
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
            Logger.Info("Loading configs...");
            string ConfigPath = Directory.GetCurrentDirectory() + @"\config.ini";


            if (!File.Exists(ConfigPath))
            {
                ConfigWorker.Write(ConfigPath);
            }
            else
            {
                ConfigWorker.Read(ConfigPath);
            }
  
            Logger.Info("Configs loaded!");

            if (IsSslEnabled && !Directory.Exists($@"{Path}\Cert"))
            {
                Directory.CreateDirectory($@"{Path}\Cert");
            }

            if (IsSslEnabled)
            {
                if ((IsSslUseCertChain && !File.Exists($@"{Path}\Cert\{SslCAName}"))
                    || !File.Exists($@"{Path}\Cert\{SslCertName}")
                    || !File.Exists($@"{Path}\Cert\{SslKeyName}"))
                {
                    Logger.Error("Cert file is missing, disabling ssl mode!");
                    IsSslEnabled = false;
                }
            }

            try
            {
                SocketWorker.Init(Port, MaxConnection);
                SocketWorker.Start();

                MySqlWorker.Open();
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
