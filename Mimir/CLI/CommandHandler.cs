using Mimir.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.CLI
{
    // Todo
    /// <summary>
    /// 控制台命令处理类
    /// </summary>
    class CommandHandler
    {
        // Todo
        /// <summary>
        /// 处理控制台命令方法
        /// </summary>
        /// <param name="args">控制台命令</param>
        public static void Handle(string[] args)
        {
            switch (args[0])
            {
                case "help":
                case "?":
                    Program.GetLogger().Info("--------------- Mimir Help ---------------");
                    Program.GetLogger().Info("help: To show this message.");
                    Program.GetLogger().Info("stop: To stop the server.");
                    Program.GetLogger().Info("user: To manage users in the database.");
                    Program.GetLogger().Info("version: To show the version of Mimir.");
                    Program.GetLogger().Info("dbinit: To initialize the database.");
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
                    Program.GetLogger().Info($"Mimir version: {Program.Version}!");
                    break;
                case "dbinit":
                    SqlProxy.Init();
                    break;
                case "ping":
                    Program.GetLogger().Info($"啪！");
                    break;
                default:
                    Program.GetLogger().Error("No such command.");
                    break;
            }
        }

        // 这个位置不好
        /// <summary>
        /// 停止程序方法
        /// </summary>
        /// <param name="exitCode">退出码</param>
        public static void Stop(int exitCode)
        {
            Poller.Stop();
            SqlProxy.Close();
            ConfigWorker.Save();
            Console.Write("Please press any key to exit Mimir.");
            Console.ReadKey(false);
            Environment.Exit(exitCode);
        }
    }
}
