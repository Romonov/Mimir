using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public class TimeWorker
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
        /// 获得13位时间戳（带偏移时间）
        /// </summary>
        /// <param name="offset">偏移的毫秒数</param>
        /// <returns>获得的13位时间戳</returns>
        public static string GetTimeStamp(long offset)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalMilliseconds) + offset).ToString();
        }

        /// <summary>
        /// 获得10位时间戳（Java格式时间戳）
        /// </summary>
        /// <returns>当前的10位时间戳</returns>
        public static string GetJavaTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalSeconds)).ToString();
        }

        /// <summary>
        /// 获得10位时间戳（Java格式时间戳，带偏移）
        /// </summary>
        /// <param name="offset">偏移的毫秒数</param>
        /// <returns>获得的10位时间戳</returns>
        public static string GetJavaTimeStamp(long offset)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return ((long)(ts.TotalSeconds) + offset).ToString();
        }
    }
}
