using Mimir.SQL;
using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimir
{
    class Poller
    {
        private static Thread thread = new Thread(Poll);
        private static Logger log = new Logger("PollThread");
        private static bool isRunning = false;

        public static void Start()
        {
            log.Info("Starting poll thread.");
            isRunning = true;
            thread.Start();
        }

        public static void Stop()
        {
            log.Info("Stopping poll thread.");
            isRunning = false;
        }

        private static void Poll()
        {
            while (isRunning)
            {
                SqlProxy.Excute("update `users` set `TryTimes` = 0");
                Thread.Sleep(60000);
            }
        }
    }
}
