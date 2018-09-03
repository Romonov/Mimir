using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.AuthServer
{
    public class Signout
    {
        public static string Process(string PostData)
        {
            JsonConvert.DeserializeObject<Request>(PostData);

            return "";
        }

        struct Request
        {
            public string username;
            public string password;
        }
    }
}
