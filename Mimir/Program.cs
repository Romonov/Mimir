using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RUL;
using System.Net;
using System.Threading;

namespace Mimir
{
    class Program
    {
        public const string Name = "Mimir";
        public const string Version = "0.0.1";

        public static string Path = Directory.GetCurrentDirectory();

        public static string ServerName = "Mimir Server";
        public static int Port = 80;
        public static int MaxConnection = 233;

        public static bool IsRunning = false;

        static void Main(string[] args)
        {
            if (!InitWorker.Init())
            {
                Logger.Error("Init failed.");
                IsRunning = false;
                Console.Read();
                return;
            }

            try
            {
                ThreadStart threadStart = new ThreadStart(Mimir.Main.Start);
                Thread thread = new Thread(threadStart);
                thread.Start();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Logger.Error("Init Failed.");
                IsRunning = false;
                Console.Read();
                return;
            }

            IsRunning = true;
            Logger.Info("Welcome!!");

            while (true)
            {
                //Console.Write(">");

                string input = Console.ReadLine();
                Logger.WriteToFile(input);
                switch (input)
                {
                    case "stop":
                        /*if (Main.Stop())
                        {
                            Logger.Info("All clients was closed!");
                            Console.Read();
                            Environment.Exit(0);
                        }*/
                        break;
                    default:
                        Logger.Error("No such command.");
                        continue;
                }
            }
        }
    }
}
