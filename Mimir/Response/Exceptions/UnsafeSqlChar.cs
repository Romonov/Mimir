using System;
using System.Text;

namespace Mimir.Response.Exceptions
{
    class UnsafeSqlChar
    {
        public static Tuple<int, string, string> GetResponse()
        {
            return new Tuple<int, string, string>(403, "text/plain", "Bad SQL Command!");
        }
    }
}
