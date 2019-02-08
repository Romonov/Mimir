using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Skins
{
    class Root
    {
        public static ValueTuple<int, string, byte[]> OnGet(Dictionary<string, string> getData)
        {

            return (200, "image/png", Encoding.Default.GetBytes(""));
        }
    }
}
