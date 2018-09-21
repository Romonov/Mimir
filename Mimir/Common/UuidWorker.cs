using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class UuidWorker
    {
        public static string GenUuid(string str)
        {
            return Guid.NewGuid().ToString("");
        }

        /*
        public static string GetPlayerUuid(string Username)
        {
            return GenUuid($"OfflinePlayer:{Username}");
        }
        */

        public static string ToUnsignedUuid(string SignedUuid)
        {
            Guid guid = new Guid("N");
            Guid.TryParse(SignedUuid, out guid);
            return guid.ToString();
        }

        public static string ToSignedUuid(string UnsignedUuid)
        {
            Guid guid = new Guid("D");
            Guid.TryParse(UnsignedUuid, out guid);
            return guid.ToString();
        }
    }
}
