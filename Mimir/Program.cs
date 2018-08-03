using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RUL;
using System.Net;

namespace Mimir
{
    class Program
    {
        public static string Path = Directory.GetCurrentDirectory();

        public static HttpListener httpListener = new HttpListener();

        static void Main(string[] args)
        {
            if (!InitWorker.Init())
            {
                Logger.Error("Init failed.");
                Console.Read();
                return;
            }

            Logger.Info("Welcome!!");
            Console.Read();
        }
    }
}
