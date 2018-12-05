using Mimir.Common.SQL;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Invalidate
    {
        public static Tuple<int, string, string> OnPost(string postData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(postData);

            DataSet dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}';");

            if (dataSetToken?.Tables[0]?.Rows?.Count >= 1)
            {
                SqlProxy.Excuter($"update `tokens` set `Status` = 0 where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}';");
            }

            return new Tuple<int, string, string>(204, "text/plain", "");
        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
        }
    }
}
