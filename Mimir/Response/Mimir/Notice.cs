using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response.Mimir
{
    public class Notice
    {
        public static ReturnContent OnGet()
        {
            // Get /mimir/notice

            ReturnContent returnContent = new ReturnContent();

            returnContent.Status = 200;

            return returnContent;
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
