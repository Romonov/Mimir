using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.Util
{
    public class VerificationCodeWorker
    {
        public static void Send(HttpContext context)
        {
            context.Session.SetString("VerificationCode", "");
        }
    }
}
