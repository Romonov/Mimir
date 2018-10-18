using Mimir.Common;
using Mimir.Response.Exceptions;
using Mimir.SQL;
using Newtonsoft.Json;
using RUL.Encrypt;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Authenticate
    {
        public static Tuple<int, string> OnPost(string PostData)
        {
            // Post /authserver/authenticate
            Response response = new Response();

            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // SQL
            DataSet dataSet = SqlProxy.Query($"SELECT * FROM users");

            // Login
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (dataRow["Email"].ToString() == request.username && dataRow["Password"].ToString() == HashWorker.MD5(request.password))
                {

                }
                else
                {
                    return InvalidPassword.GetResponse();
                }
            }

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

            // Profiles
            PlayerProfiles profiles = new PlayerProfiles();
            profiles.id = UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid());
            profiles.name = "Romonov";
            response.selectedProfile = profiles;

            // Users
            if (request.requestUser)
            {
                User user = new User();
                user.id = "Romonov";
                Properties properties = new Properties();
                properties.name = "preferredLanguage";
                properties.value = "zh_CN";
                user.properties = new Properties[] { properties };
                response.user = user;
            }

            return new Tuple<int, string>(200, JsonConvert.SerializeObject(response));
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
            public PlayerProfiles selectedProfile;
            public User? user;
        }
        
        struct PlayerProfiles
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
