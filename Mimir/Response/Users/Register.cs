using Mimir.Common;
using Mimir.Common.SQL;
using Mimir.Response.Exceptions;
using RUL.Net;
using RUL.Encrypt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.Users
{
    class Register
    {
        public static Tuple<int, string, string> OnPost(string PostData)
        {
            // Post /users/register
            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // 检查用户名或邮箱重复
            if (!SqlSecurity.Check(PostData))
            {
                return UnsafeSqlChar.GetResponse();
            }

            if (!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `users` where `Username` = '{request.Username}'")))
            {
                return new Tuple<int, string, string>(200, "text/plain", "ExceptionUsernameAlreadyUsed!");
            }

            if (!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `users` where `Email` = '{request.Email}'")))
            {
                return new Tuple<int, string, string>(200, "text/plain", "ExceptionEmailAlreadyUsed!");
            }

            string UUID = UuidWorker.GenUuid();

            SqlProxy.Excuter($"insert into `users` (`UUID`, `Username`, `Password`, `Email`, `Nickname`, `PreferredLanguage`, `LastLogin`, `CreateTime`, `IsLogged`, `IsAdmin`) values ('{UUID}', '{request.Username}', '{HashWorker.MD5(request.Password)}', '{request.Email}', '{request.Nickname}', '{request.PreferredLanguage}', '{TimeWorker.GetTimeStamp()}', '{TimeWorker.GetTimeStamp()}', '1', '{request.IsAdmin}');");

            return new Tuple<int, string, string>(200, "text/plain", UUID);
        }

        struct Request
        {
            public string Username;
            public string Password;
            public string Nickname;
            public string Email;
            public string PreferredLanguage;
            public int IsAdmin;
        }
    }
}
