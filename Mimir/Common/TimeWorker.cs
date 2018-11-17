using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class TimeWorker
    {
        public static string GetTimeStamp()
        {
            TimeSpan ts  = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ts.TotalMilliseconds.ToString();
        }
    }
}
