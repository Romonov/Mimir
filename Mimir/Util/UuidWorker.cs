using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// Uuid工具类
    /// </summary>
    public class UuidWorker
    {
        /// <summary>
        /// 获得一个无符号Uuid
        /// </summary>
        /// <returns>无符号Uuid</returns>
        public static string GetUuid()
        {
            return Guid.NewGuid().ToString("");
        }

        /// <summary>
        /// 通过给定的Bytes获得一个无符号Uuid（用于兼容Java）
        /// </summary>
        /// <param name="bytes">给定的Bytes参数</param>
        /// <returns>无符号Uuid</returns>
        public static string GetUuid(byte[] bytes)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytes);
            hash[6] &= 0x0f;
            hash[6] |= 0x30;
            hash[8] &= 0x3f;
            hash[8] |= 0x80;
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// 把Uuid转为无符号Uuid
        /// </summary>
        /// <param name="SignedUuid">Uuid</param>
        /// <returns>无符号Uuid</returns>
        public static string ToUnsignedUuid(string SignedUuid)
        {
            Guid guid = new Guid();
            Guid.TryParse(SignedUuid, out guid);
            return guid.ToString("N");
        }

        /// <summary>
        /// 把无符号Uuid转为Uuid
        /// </summary>
        /// <param name="UnsignedUuid">无符号Uuid</param>
        /// <returns>Uuid</returns>
        public static string ToSignedUuid(string UnsignedUuid)
        {
            Guid guid = new Guid();
            Guid.TryParse(UnsignedUuid, out guid);
            return guid.ToString("D");
        }
    }
}
