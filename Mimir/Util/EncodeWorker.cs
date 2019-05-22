using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// 转码工具类
    /// </summary>
    public class EncodeWorker
    {
        /// <summary>
        /// Base64转码
        /// </summary>
        /// <param name="str">要转码的字符串</param>
        /// <returns>转码后的字符串</returns>
        public static string Base64Encoder(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string Base64Decoder(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
