using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Util
{
    class UuidWorker
    {
        public static string GenUuid()
        {
            return Guid.NewGuid().ToString("");
        }

        public static string ToUnsignedUuid(string SignedUuid)
        {
            Guid guid = new Guid();
            Guid.TryParse(SignedUuid, out guid);
            return guid.ToString("N");
        }

        public static string ToSignedUuid(string UnsignedUuid)
        {
            Guid guid = new Guid();
            Guid.TryParse(UnsignedUuid, out guid);
            return guid.ToString("D");
        }
    }
}
