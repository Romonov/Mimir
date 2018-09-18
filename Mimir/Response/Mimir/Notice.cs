using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Mimir
{
    public class Notice
    {
        public static string OnGet()
        {
            // Get /mimir/notice



            return "";
        }

        struct Notices
        {
            public notice[] Notice;
        }

        struct notice
        {
            public string Title;
            public string Details;
            public string Link;
        }
    }
}
