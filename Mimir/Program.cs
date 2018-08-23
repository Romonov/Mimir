using Mimir.Common;
using Mimir.SQL;
using RUL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class Program
    {
        public const string Version = "1.0.0";

        public static int Port = 45679;
        public static int MaxConnection = 233;

        public static bool isRunning = false;

        public static string SQLIP = "localhost";
        public static string SQLUsername = "root";
        public static string SQLPassword = "123456";
        public const string SQLDatabase = "mimir";

        SocketWorker SocketWorker = new SocketWorker();

        public static X509Certificate serverCertificate = new X509Certificate();

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
                    INI.Write(configPath, "General", "Port", Port.ToString());
                    INI.Write(configPath, "General", "MaxConnection", MaxConnection.ToString());

                    INI.Write(configPath, "SQL", "IP", SQLIP);
                    INI.Write(configPath, "SQL", "Username", SQLUsername);
                    INI.Write(configPath, "SQL", "Password", SQLPassword);
                }
                else
                {
                    Port = int.Parse(INI.Read(configPath, "General", "Port"));
                    MaxConnection = int.Parse(INI.Read(configPath, "General", "MaxConnection"));

                    SQLIP = INI.Read(configPath, "SQL", "IP");
                    SQLUsername = INI.Read(configPath, "SQL", "Username");
                    SQLPassword = INI.Read(configPath, "SQL", "Password");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                File.Delete(configPath);

                INI.Write(configPath, "General", "Port", Port.ToString());
                INI.Write(configPath, "General", "MaxConnection", MaxConnection.ToString());

                INI.Write(configPath, "SQL", "IP", SQLIP);
                INI.Write(configPath, "SQL", "Username", SQLUsername);
                INI.Write(configPath, "SQL", "Password", SQLPassword);

                return false;
            }
            finally
            {
                Logger.Info("Configs loaded!");
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

            isRunning = true;

            return true;
        }
    }
}
