using Mimir.Response.Exceptions;
using Mimir.Common.SQL;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Validate
    {
        public static Tuple<int, string, string> OnPost(string postData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(postData);

            // Tokens
            DataSet dataSetToken = SqlProxy.Query("SELECT * FROM `tokens`");

            DataSet dataSetUser = SqlProxy.Query("SELECT * FROM `users`");

            DataRow dataRowToken = null;
            DataRow dataRowUser = null;

            foreach (DataRow dataRow in dataSetToken.Tables[0].Rows)
            {
                if (dataRow["AccessToken"].ToString() == request.accessToken)
                {
                    if (request.clientToken != null)
                    {
                        if (dataRow["ClientToken"].ToString() == request.clientToken)
                        {
                            return InvalidToken.GetResponse();
                        }
                    }

                    dataRowToken = dataRow;
                }
                else
                {
                    return InvalidToken.GetResponse();
                }
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
