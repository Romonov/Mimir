using Newtonsoft.Json;
using System;

namespace Mimir.Response.AuthServer
{
    public class Invalidate
    {
        public static Tuple<int, string, string> OnPost(string PostData)
        {
            JsonConvert.DeserializeObject<Request>(PostData);

            return new Tuple<int, string, string>(204, "text/plain", "");
        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
        }
    }
}
