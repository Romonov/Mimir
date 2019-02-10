using RUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.API
{
    class Root
    {
        private static Logger log = new Logger("API");

        public static Logger GetLogger()
        {
            return log;
        }
    }
}
