using Mimir.Response.Exceptions;
using Mimir.Common.SQL;
using Newtonsoft.Json;
using RUL.Encrypt;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Signout
    {
        public static Tuple<int, string, string> OnPost(string PostData)
        {
            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // Login
            DataSet dataSetUser = SqlProxy.Query($"SELECT * FROM users");

            DataRow userRow = null;
            
            foreach (DataRow dataRow in dataSetUser.Tables[0].Rows)
            {
                if (dataRow["Email"].ToString() == request.username && dataRow["Password"].ToString() == HashWorker.MD5(request.password))
                {
                    userRow = dataRow;
                }
                else
                {
                    return InvalidPassword.GetResponse();
                }
            }

            // Signout
            SqlProxy.Excuter($"UPDATE `tokens` SET `Status` = 2 WHERE `BindUser` = {userRow["Username"].ToString()}");

            return new Tuple<int, string, string>(204, "text/plain", "");
        }

        struct Request
        {
            public string username;
            public string password;
        }
    }
}
