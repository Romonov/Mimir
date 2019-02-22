using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// Uuid工具类
    /// </summary>
    class UuidWorker
    {
        /// <summary>
        /// 生成一个无符号Uuid
        /// </summary>
        /// <returns></returns>
        public static string GenUuid()
        {
            return Guid.NewGuid().ToString("");
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
