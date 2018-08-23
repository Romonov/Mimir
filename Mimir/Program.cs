using System;
using System.Collections.Generic;
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

        public static X509Certificate serverCertificate = new X509Certificate();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome!!");
        }
    }
}
