using System;

namespace Mimir.Util
{
    class TimeWorker
    {
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalMilliseconds)).ToString();
        }

        public static string GetTimeStamp(long add)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalMilliseconds) + add).ToString();
        }

        public static string GetJavaTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalSeconds)).ToString();
        }
    }
}
