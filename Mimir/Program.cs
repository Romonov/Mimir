using Mimir.Common;
using Mimir.SQL;
using RUL;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Mimir
{
    class Program
    {
        public const string Name = "Mimir";
        public const string Version = "0.3.0";

        public static string Path = Directory.GetCurrentDirectory();

        public static int Port = 443;
        public static int MaxConnection = 233;

        public static bool IsRunning = false;

        public static bool UseSsl = true;
        public static bool SslUseCABundle = true;

        public static string SslCAName = "ca_bundle.crt";
        public static string SslCAContent = "";
        public static string SslCertName = "certificate.crt";
        public static string SslCertContent = "";
        public static string SslKeyName = "private.key";
        public static string SslKeyContent = "";
        public static string SslPfxName = "ssl.pfx";
        public static string SslPfxPassword = "123";

        public static string SQLIP = "localhost";
        public static string SQLUsername = "root";
        public static string SQLPassword = "123456";
        public const string SQLDatabase = "mimir";

        SocketWorker SocketWorker = new SocketWorker();

        public static X509Certificate serverCertificate = new X509Certificate();

        // todo: Read/Write config file!
        public static string PublicKey = "";
        public static string[] SkinDomains = new string[1] { "*.romonov.com" };
        public static string ServerName = "Mimir Example Server";

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

        bool Init()
        {
            Logger.Info("Loading configs...");
            string configPath = Directory.GetCurrentDirectory() + @"\config.ini";

            try
            {
                if (!File.Exists(configPath))
                {
                    INIWorker.Write();
                }
                else
                {
                    INIWorker.Read();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                File.Delete(configPath);

                INIWorker.Write();

                return false;
            }
            finally
            {
                Logger.Info("Configs loaded!");
            }

            if (UseSsl && !Directory.Exists($@"{Path}\Cert"))
            {
                Directory.CreateDirectory($@"{Path}\Cert");
            }

            if (UseSsl)
            {
                if ((SslUseCABundle && !File.Exists($@"{Path}\Cert\{SslCAName}"))
                    || !File.Exists($@"{Path}\Cert\{SslCertName}")
                    || !File.Exists($@"{Path}\Cert\{SslKeyName}"))
                {
                    Logger.Error("Cert file is missing, disabling ssl mode!");
                    UseSsl = false;
                }
                else
                {
                    //serverCertificate = new X509Certificate($@"F:\Mimir\Mimir\Mimir\bin\Debug\Cert\{SslPfxName}", "123");
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
