using Mimir.Response.Exceptions;
using Newtonsoft.Json;
using RUL.Encrypt;
using System;
using System.Data;
using Mimir.SQL;

namespace Mimir.Response.AuthServer
{
    public class Signout
    {
        public static ValueTuple<int, string, string> OnPost(string PostData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // Login
            DataSet dataSetUser = SqlProxy.Query($"select * from `users` where `Username` = '{SqlSecurity.Parse(request.username)}' and `Password` = '{HashWorker.MD5(request.password)}' and `TryTimes` <= {Program.UserTryTimesPerMinutes};");

            SqlProxy.Excute($"update `users` set `TryTimes` = `TryTimes` + 1 where `Email` = '{SqlSecurity.Parse(request.username)}'");

            DataRow dataRowUser;

            if (dataSetUser?.Tables[0]?.Rows.Count >= 1)
            {
                dataRowUser = dataSetUser.Tables[0].Rows[0];
            }
            else
            {
                return ForbiddenOperation.GetResponse();
            }

            // Signout
            SqlProxy.Excute($"update `tokens` set `Status` = 0 where `BindUser` = {dataRowUser["Username"].ToString()}");

            return (204, "text/plain", "");
        }

        struct Request
        {
            public string username;
            public string password;
        }
    }
}
