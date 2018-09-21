using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response.Users
{
    class LogOut
    {
        public static ReturnContent OnPost(string msg)
        {
            // Post /users/logout
            ReturnContent returnContect = new ReturnContent();

            return returnContect;
        }
    }
}
