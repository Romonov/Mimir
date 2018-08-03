using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir
{
    class InitWorker
    {
        public static bool Init()
        {
            if (File.Exists(Program.Path + @"/config.ini"))
            {

            }
            return true;
        }
    }
}
