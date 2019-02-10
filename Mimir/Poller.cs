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
        private static Thread threadUsersTrytimes = new Thread(PollUsersTrytimes);
        private static Thread threadSecurityRegisterTimes = new Thread(PollSecurityRegisterTimes);
        private static Thread threadIPSecurity = new Thread(PollIPSecurity);

        private static Logger log = new Logger("PollThread");
        private static bool isRunning = false;

        public static void Start()
        {
            log.Info("Starting poll thread.");
            isRunning = true;
            threadUsersTrytimes.Start();
        }

        public static void Stop()
        {
            log.Info("Stopping poll thread.");
            isRunning = false;
        }

        private static void PollUsersTrytimes()
        {
            while (isRunning)
            {
                SqlProxy.Excute("update `users` set `TryTimes` = 0");
                Thread.Sleep(60000);
            }
        }

        private static void PollSecurityRegisterTimes()
        {
            while (isRunning)
            {
                Program.SecurityRegisterTimes = 0;
                Thread.Sleep(60000);
            }
        }

        private static void PollIPSecurity()
        {
            while (isRunning)
            {
                Program.IPSecurity.Clear();
                Thread.Sleep(60000);
            }
        }
    }
}
