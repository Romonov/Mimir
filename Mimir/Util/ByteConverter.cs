using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// Byte和对象互转工具类
    /// </summary>
    public class ByteConverter
    {
        /// <summary>
        /// 将对象转换为Byte数组
        /// </summary>
        /// <param name="obj">被转换对象</param>
        /// <returns>转换后Byte数组</returns>
        public static byte[] ToBytes(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] serializedResult = Encoding.UTF8.GetBytes(json);
            return serializedResult;
        }

        /// <summary>
        /// 将Byte数组转换成对象
        /// </summary>
        /// <param name="buffer">被转换Byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static object ToObject(byte[] buffer)
        {
            string json = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<object>(json);
        }

        /// <summary>
        /// 将Byte数组转换成对象
        /// </summary>
        /// <param name="buffer">被转换Byte数组</param>
        /// <returns>转换完成后的对象</returns>
        public static T ToObject<T>(byte[] buffer)
        {
            string json = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
