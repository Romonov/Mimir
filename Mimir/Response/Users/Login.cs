using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Users
{
    class Login
    {
        public static Tuple<int, string> OnPost(string PostData)
        {
            // Post /users/login
            Request request = JsonConvert.DeserializeObject<Request>(PostData);



            return new Tuple<int, string>(200, "true");
        }

        struct Request
        {
            public string Username;
            public string Password;
        }
    }
}
