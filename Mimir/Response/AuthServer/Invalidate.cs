using Mimir.SQL;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Invalidate
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(postData);

            DataSet dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}';");

            if (dataSetToken?.Tables[0]?.Rows?.Count >= 1)
            {
                SqlProxy.Excute($"update `tokens` set `Status` = 0 where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}';");
            }

            return (204, "text/plain", "");
        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
        }
    }
}
