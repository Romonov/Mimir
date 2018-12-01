using Newtonsoft.Json;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Users
{
    class LogOut
    {

        public static Tuple<int, string, string> OnPost(string PostData)
        {
            // Post /users/logout
            JsonConvert.DeserializeObject<HttpReq>(PostData);

            return new Tuple<int, string, string>(204, "text/plain", "");
        }
    }
}
