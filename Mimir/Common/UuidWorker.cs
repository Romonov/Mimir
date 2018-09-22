using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class UuidWorker
    {
        /// <summary>
        /// 生成UUID
        /// </summary>
        /// <returns>生成的UUID</returns>
        public static string GenUuid()
        {
            return Guid.NewGuid().ToString("");
        }

        /// <summary>
        /// 转换为无符号UUID
        /// </summary>
        /// <param name="SignedUuid">UUID</param>
        /// <returns>无符号UUID</returns>
        public static string ToUnsignedUuid(string SignedUuid)
        {
            Guid guid = new Guid("N");
            Guid.TryParse(SignedUuid, out guid);
            return guid.ToString();
        }

        /// <summary>
        /// 转换为UUID
        /// </summary>
        /// <param name="UnsignedUuid">无符号UUID</param>
        /// <returns>UUID</returns>
        public static string ToSignedUuid(string UnsignedUuid)
        {
            Guid guid = new Guid("D");
            Guid.TryParse(UnsignedUuid, out guid);
            return guid.ToString();
        }
    }
}
