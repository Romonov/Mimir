using System;

namespace Mimir.Util
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    class TimeWorker
    {
        /// <summary>
        /// 获得13位时间戳
        /// </summary>
        /// <returns>当前的13位时间戳</returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalMilliseconds)).ToString();
        }

        /// <summary>
        /// 获得13位时间戳
        /// </summary>
        /// <param name="add">增加的毫秒数</param>
        /// <returns>获得13位时间戳</returns>
        public static string GetTimeStamp(long add)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalMilliseconds) + add).ToString();
        }

        /// <summary>
        /// 获得10位时间戳
        /// </summary>
        /// <returns>当前的10位时间戳</returns>
        public static string GetJavaTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalSeconds)).ToString();
        }
    }
}
