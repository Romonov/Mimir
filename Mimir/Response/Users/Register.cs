using RUL.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response.Users
{
    class Register
    {
        public static ReturnContent OnPost(string msg)
        {
            // Post /users/register
            ReturnContent returnContect = new ReturnContent();

            return returnContect;
        }
    }
}
