using System;
using System.Text;

namespace Mimir.Response.Exceptions
{
    class UnsafeSqlChar
    {
        public static ValueTuple<int, string, string> GetResponse()
        {
            return (403, "text/plain", "Bad SQL Command!");
        }
    }
}
