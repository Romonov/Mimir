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
            if (getData != null && getData.ContainsKey("id"))
            {

            }
            if (getData != null && getData.ContainsKey("uuid"))
            {

            }
            return (200, "image/png", Encoding.Default.GetBytes(""));
        }
    }
}
