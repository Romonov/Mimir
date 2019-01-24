using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.CLI
{
    class CommandHandler
    {
        public static void Handle(string[] args)
        {
            switch (args[0])
            {
                case "help":
                case "?":
                    Program.GetLogger().Info("--------------- Mimir Help ---------------");
                    Program.GetLogger().Info("/help: To show this message.");
                    Program.GetLogger().Info("/stop: To stop the server.");
                    Program.GetLogger().Info("/user: To manage users in the database.");
                    Program.GetLogger().Info("/version: To show the version of Mimir.");
                    Program.GetLogger().Info("--------------- [ 1 / 1 ]  ---------------");
                    break;
                case "stop":
                    Stop(0);
                    break;
                case "user":
                    break;
                case "version":
                case "ver":
                case "v":
                    break;
                default:
                    Program.GetLogger().Error("No such command.");
                    break;
            }
        }

        public static void Stop(int exitCode)
        {
            ConfigWorker.Save($@"{Program.Path}\config.ini");
            Console.Write("Please press any key to exit Mimir.");
            Console.ReadKey(false);
            Environment.Exit(exitCode);
        }
    }
}
