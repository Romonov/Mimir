using RUL.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response.Users
{
    class Login
    {
        public static ReturnContent OnPost(string msg)
        {
            // Post /users/login
            ReturnContent returnContect = new ReturnContent();

            return returnContect;
        }
    }
}
