using Mimir.Common.SQL;
using Mimir.Response.Exceptions;
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
        public static Tuple<int, string> OnPost(string PostData)
        {
            // Post /users/register
            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // 检查用户名或邮箱重复
            if (!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `users` where `Username` = '{request.Username}'")))
            {
                return UsernameAlreadyUsed.GetResponse();
            }

            if (!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `users` where `Email` = '{request.Email}'")))
            {
                return EmailAlreadyUsed.GetResponse();
            }

            SqlProxy.Excuter("insert into `users` ");

            return new Tuple<int, string>(200, "UUID");
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
