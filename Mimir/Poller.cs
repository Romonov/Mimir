using Mimir.SQL;
using RUL;
using System.Threading;

namespace Mimir
{
    /// <summary>
    /// 轮询管理类（这个实现不好）
    /// </summary>
    class Poller
    {
        private static Thread threadUsersTrytimes = new Thread(PollUsersTrytimes);
        private static Thread threadSecurityRegisterTimes = new Thread(PollSecurityRegisterTimes);
        private static Thread threadIPSecurity = new Thread(PollIPSecurity);

        private static Logger log = new Logger("PollThread");
        private static bool isRunning = false;

        /// <summary>
        /// 启动轮询线程
        /// </summary>
        public static void Start()
        {
            log.Info("Starting poll thread.");
            isRunning = true;
            threadUsersTrytimes.Start();
        }

        /// <summary>
        /// 停止轮询线程
        /// </summary>
        public static void Stop()
        {
            log.Info("Stopping poll thread.");
            isRunning = false;
        }

        /// <summary>
        /// 每分钟每个用户尝试登录次数
        /// </summary>
        private static void PollUsersTrytimes()
        {
            while (isRunning)
            {
                SqlProxy.Excute("update `users` set `TryTimes` = 0");
                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// 每分钟全局注册次数
        /// </summary>
        private static void PollSecurityRegisterTimes()
        {
            while (isRunning)
            {
                Program.SecurityRegisterTimes = 0;
                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// 每分钟每IP请求次数
        /// </summary>
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
