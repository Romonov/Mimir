using Mimir.Response.Exceptions;
using Newtonsoft.Json;
using System;
using System.Data;
using RUL;
using Mimir.SQL;
using Mimir.Util;

namespace Mimir.Response.AuthServer
{
    public class Refresh
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            // Post /authserver/refresh
            Response response = new Response();

            Request request = JsonConvert.DeserializeObject<Request>(postData);

            // Tokens
            DataSet dataSetToken;
            DataRow dataRowOldToken;
            DataRow dataRowUser;

            if (request.clientToken != null)
            {
                dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}' and `ClientToken` = '{SqlSecurity.Parse(request.clientToken)}' and `Status` >= 1;");
            }
            else
            {
                dataSetToken = SqlProxy.Query($"select * from `tokens` where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}' and `Status` >= 1;");
            }

            if (dataSetToken.Tables[0].Rows.Count >= 1)
            {
                dataRowOldToken = dataSetToken.Tables[0].Rows[0];
                SqlProxy.Query($"update `tokens` set `Status` = 0 where `AccessToken` = '{SqlSecurity.Parse(request.accessToken)}'");
            }
            else
            {
                return InvalidToken.GetResponse();
            }

            if (request.clientToken != null)
            {
                response.clientToken = request.clientToken;
            }
            else
            {
                response.clientToken = UuidWorker.GenUuid();
            }

            response.accessToken = UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid());

            // Users
            dataRowUser = SqlProxy.Query($"select * from `users` where `Username` = '{dataRowOldToken["BindUser"]}';").Tables[0].Rows[0];

            if (request.requestUser)
            {
                
                User user = new User();
                user.id = dataRowUser["Username"].ToString();
                Properties properties = new Properties();
                properties.name = "preferredLanguage";
                properties.value = dataRowUser["PreferredLanguage"].ToString();
                user.properties = new Properties[] { properties };
                response.user = user;
            }

            // Profiles
            if (request.selectedProfile.HasValue)
            {
                response.selectedProfile = request.selectedProfile;
                SqlProxy.Excute($"insert into `tokens` (`AccessToken`, `ClientToken`, `BindProfile`, `CreateTime`, `Status`, `BindUser`) VALUES('{response.accessToken}', '{SqlSecurity.Parse(response.clientToken)}', '{SqlSecurity.Parse(response.selectedProfile.Value.name)}', '{Time.GetUnixTimeStamp()}', 2, '{dataRowUser["Username"].ToString()}');");
                SqlProxy.Excute($"update `profiles` set `IsSelected` = 0 where `UserID` = '{dataRowUser["Username"].ToString()}';");
                SqlProxy.Excute($"update `profiles` set `IsSelected` = 1 where `Name` = '{request.selectedProfile.Value.name}';");
            }
            else
            {
                SqlProxy.Excute($"insert into `tokens` (`AccessToken`, `ClientToken`, `CreateTime`, `Status`, `BindUser`) VALUES('{response.accessToken}', '{SqlSecurity.Parse(response.clientToken)}', '{Time.GetUnixTimeStamp()}', 2, '{dataRowUser["Username"].ToString()}');");
            }
            
            return (200, "text/plain", JsonConvert.SerializeObject(response));


        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
            public bool requestUser;
            public SelectedProfile? selectedProfile;
        }
        struct Response
        {
            public string accessToken;
            public string clientToken;
            public SelectedProfile? selectedProfile;
            public User? user;
        }
        struct SelectedProfile
        {
            public string id;
            public string name;
            public Properties[] properties;
        }
        struct User
        {
            public string id;
            public Properties[] properties;
        }
        struct Properties
        {
            public string name;
            public string value;
            public string signature;
        }
    }
}
