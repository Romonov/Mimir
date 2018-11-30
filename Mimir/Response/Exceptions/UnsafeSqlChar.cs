using System;

namespace Mimir.Response.Exceptions
{
    class UnsafeSqlChar
    {
        public static Tuple<int, string> GetResponse()
        {
            return new Tuple<int, string>(403, "Bad SQL Command!");
        }
    }
}
