using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.AuthServer
{
    class Root
    {
        private static Logger log = new Logger("AuthServer");

        public static Logger GetLogger()
        {
            return log;
        }
    }
}
