using Mimir.Response.Exceptions;
using Mimir.Common.SQL;
using Newtonsoft.Json;
using RUL.Encrypt;
using System;
using System.Data;
using System.Text;

namespace Mimir.Response.AuthServer
{
    public class Signout
    {
        public static Tuple<int, string, string> OnPost(string PostData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // Login
            DataSet dataSetUser = SqlProxy.Query($"select * from `users` where `Username` = '{SqlSecurity.Parse(request.username)}' and `Password` = '{HashWorker.MD5(request.password)}';");
            DataRow dataRowUser;

            if (dataSetUser?.Tables[0]?.Rows.Count >= 1)
            {
                dataRowUser = dataSetUser.Tables[0].Rows[0];
            }
            else
            {
                return InvalidPassword.GetResponse();
            }

            // Signout
            SqlProxy.Excuter($"update `tokens` set `Status` = 0 where `BindUser` = {dataRowUser["Username"].ToString()}");

            return new Tuple<int, string, string>(204, "text/plain", "");
        }

        struct Request
        {
            public string username;
            public string password;
        }
    }
}
