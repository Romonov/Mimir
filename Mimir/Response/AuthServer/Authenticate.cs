using Mimir.Response.Exceptions;
using Mimir.SQL;
using Mimir.Util;
using Newtonsoft.Json;
using RUL.Encrypt;
using System;
using System.Collections.Generic;
using System.Data;

namespace Mimir.Response.AuthServer
{
    class Authenticate
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            // Post /authserver/authenticate
            Response response = new Response();

            Request request = JsonConvert.DeserializeObject<Request>(postData);

            // Login
            DataSet dataSetUser = SqlProxy.Query($"select * from `users` where `Email` = '{SqlSecurity.Parse(request.username)}' and `Password` = '{HashWorker.MD5(request.password)}' and `TryTimes` <= {Program.UserLoginTryTimesPerMinute};");

            SqlProxy.Excute($"update `users` set `TryTimes` = `TryTimes` + 1 where `Email` = '{SqlSecurity.Parse(request.username)}'");
            SqlProxy.Excute($"update `users` set `LastLogin` = {TimeWorker.GetTimeStamp()} where `Email` = '{SqlSecurity.Parse(request.username)}'");

            DataRow userRow;

            if (dataSetUser.Tables[0].Rows.Count == 1)
            {
                userRow = dataSetUser.Tables[0].Rows[0];
            }
            else
            {
                return ForbiddenOperation.GetResponse();
            }

            

            // Profiles
            DataSet dataSetProfiles = SqlProxy.Query($"SELECT * FROM `profiles` where `UserID` = '{userRow["Username"].ToString()}';");

            List<PlayerProfiles> availableProfiles = new List<PlayerProfiles>();

            foreach (DataRow dataRow in dataSetProfiles.Tables[0].Rows)
            {
                if (dataRow["UserID"].ToString() == userRow["Username"].ToString())
                {
                    PlayerProfiles playerProfile = new PlayerProfiles();
                    playerProfile.id = dataRow["UnsignedUUID"].ToString();
                    playerProfile.name = dataRow["Name"].ToString();

                    if (dataRow["IsSelected"].ToString() == "True")
                    {
                        response.selectedProfile = playerProfile;
                    }
                    availableProfiles.Add(playerProfile);
                }
            }

            PlayerProfiles[] availableProfilesFinal = availableProfiles.ToArray();
            response.availableProfiles = availableProfilesFinal;

            // Tokens
            response.accessToken = UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid());

            if (request.clientToken == null)
            {
                response.clientToken = UuidWorker.GenUuid();
            }
            else
            {
                response.clientToken = request.clientToken;
            }

            if (response.selectedProfile.HasValue)
            {
                SqlProxy.Excute($"insert into `tokens` (`AccessToken`, `ClientToken`, `BindProfile`, `CreateTime`, `Status`, `BindUser`) VALUES('{SqlSecurity.Parse(response.accessToken)}', '{SqlSecurity.Parse(response.clientToken)}', '{SqlSecurity.Parse(response.selectedProfile.Value.name)}', '{TimeWorker.GetTimeStamp()}', 2, '{userRow["Username"].ToString()}');");
            }
            else
            {
                SqlProxy.Excute($"insert into `tokens` (`AccessToken`, `ClientToken`, `CreateTime`, `Status`, `BindUser`) VALUES ('{SqlSecurity.Parse(response.accessToken)}', '{SqlSecurity.Parse(response.clientToken)}', '{TimeWorker.GetTimeStamp()}', 2, '{userRow["Username"].ToString()}');");
            }

            // Users
            if (request.requestUser)
            {
                User user = new User();
                user.id = userRow["Username"].ToString();
                Properties properties = new Properties();
                properties.name = "preferredLanguage";
                properties.value = userRow["PreferredLanguage"].ToString();
                user.properties = new Properties?[] { properties };
                response.user = user;
            }

            return (200, "text/plain", JsonConvert.SerializeObject(response));
        }

        struct Request
        {
            public string username;
            public string password;
            public string clientToken;
            public bool requestUser;
            public Agent agent;
        }
        struct Agent
        {
            public string name;
            public int version;
        }
        struct Response
        {
            public string accessToken;
            public string clientToken;
            public PlayerProfiles[] availableProfiles;
            public PlayerProfiles? selectedProfile;
            public User? user;
        }
        struct PlayerProfiles
        {
            public string id;
            public string name;
            public Properties?[] properties;
        }
        struct User
        {
            public string id;
            public Properties?[] properties;
        }
        struct Properties
        {
            public string name;
            public string value;
            public string signature;
        }
    }
}
