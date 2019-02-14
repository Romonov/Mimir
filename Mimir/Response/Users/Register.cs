using Mimir.SQL;
using Mimir.Util;
using RUL.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mimir.Response.Users
{
    class Register
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            ++Program.SecurityRegisterTimes;
            if (Program.SecurityRegisterTimes > Program.SecurityRegisterTimesPerMinute)
            {
                return (429, "text/plain", "Too many requests.");
            }

            string originData = HttpUtility.UrlDecode(postData, Encoding.Default);
            string[] splitedData = originData.Split('&');
            Dictionary<string, string> regData = new Dictionary<string, string>();
            foreach (string data in splitedData)
            {
                string[] tmp = data.Split('=');
                if (tmp.Length == 2)
                {
                    regData.Add(tmp[0], tmp[1]);
                }
            }

            if ((!regData.ContainsKey("username")) 
                || (!regData.ContainsKey("password"))
                || (!regData.ContainsKey("repeat_password"))
                || (!regData.ContainsKey("email")) 
                || (!regData.ContainsKey("nickname")) 
                || (!regData.ContainsKey("profile")))
            {
                return (403, "text/plain", "Bad operation.");
            }

            if (regData["password"] != regData["repeat_password"])
            {
                return (200, "text/html", "Password not match.");
            }

            if ((!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `users` where `username` = '{SqlSecurity.Parse(regData["username"].ToLower())}' or `Email` = '{SqlSecurity.Parse(regData["email"])}';")))
                || (!SqlProxy.IsEmpty(SqlProxy.Query($"select * from `profiles` where `Name` = '{SqlSecurity.Parse(regData["profile"])}';"))))
            {
                return (200, "text/html", "Something repeated.");
            }

            if (int.Parse(SqlProxy.Query("select count(1) from `users`").Tables[0].Rows[0]["Count(1)"].ToString()) > Program.UserMaxRegistration)
            {
                return (200, "text/html", "Too many users.");
            }

            SqlProxy.Excute($"insert into `users` (`UUID`, `Username`, `Password`, `Email`, `Nickname`, `PreferredLanguage`, `LastLogin`, `CreateTime`) " +
                $"value ('{UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid())}', '{SqlSecurity.Parse(regData["username"].ToLower())}', '{SqlSecurity.Parse(HashWorker.MD5(regData["password"]))}', '{SqlSecurity.Parse(regData["email"])}', '{SqlSecurity.Parse(regData["nickname"])}', 'zh_CN', '{TimeWorker.GetTimeStamp()}', '{TimeWorker.GetTimeStamp()}');");
            SqlProxy.Excute($"insert into `profiles` (`UserID`, `Name`, `UnsignedUUID`, `IsSelected`) " +
                $"value ('{SqlSecurity.Parse(regData["username"].ToLower())}', '{SqlSecurity.Parse(regData["profile"])}', '{UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid())}', 1);");

            Root.GetLogger().Info($"User {regData["username"]} with profile {regData["profile"]} was successfully registered.");

            return (200, "text/html", "Register successful!");
        }
    }
}
