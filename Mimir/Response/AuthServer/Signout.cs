using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimir.Common.Processor;

namespace Mimir.Response.AuthServer
{
    public class Signout
    {
        public static ReturnContent OnPost(string PostData)
        {
            ReturnContent returnContect = new ReturnContent();

            JsonConvert.DeserializeObject<Request>(PostData);

            return returnContect;
        }

        struct Request
        {
            public string username;
            public string password;
        }
    }
}
