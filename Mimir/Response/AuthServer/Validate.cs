using Mimir.Response.Exceptions;
using Mimir.SQL;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Validate
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            // Post /authserver/validate

            Request request = JsonConvert.DeserializeObject<Request>(postData);

            // Tokens
            DataSet dataSetToken;

            if (request.clientToken != null)
            {
                dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}' and `ClientToken` = '{SqlSecurity.Parse(request.clientToken)}' and `Status` = 2;");
            }
            else
            {
                dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}' and `Status` = 2;");
            }

            if (dataSetToken.Tables[0].Rows.Count >= 1)
            {
                return (204, "text/plain", "");
            }
            else
            {
                return InvalidToken.GetResponse();
            }
        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
        }
    }
}
