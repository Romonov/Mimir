using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.SessionServer
{
    class Root
    {
        private static Logger log = new Logger("SessionServer");

        public static Logger GetLogger()
        {
            return log;
        }
    }
}
