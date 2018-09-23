using Mimir.Common;
using Mimir.SQL;
using Newtonsoft.Json;
using System.Data;
using static Mimir.Common.Processor;

namespace Mimir.Response.AuthServer
{
    public class Authenticate
    {
        public static ReturnContent OnPost(string PostData)
        {
            // Post /authserver/authenticate
            ReturnContent returnContect = new ReturnContent();
            Response response = new Response();

            Request request = JsonConvert.DeserializeObject<Request>(PostData);
            
            DataSet dataSet = SqlProxy.Querier($"SELECT users.Email FROM users WHERE users.Email = '{request.username}'");
            DataRow[] dataRows = dataSet.Tables[0].Select();

            // Passwords
            if (request.password != "123456") //dataRows[0]["password"].ToString())
            {
                returnContect.Status = 403;
                ReturnError returnError = new ReturnError();
                returnError.error = "ForbiddenOperationException";
                returnError.errorMessage = "Invalid credentials. Invalid username or password.";
                returnContect.Contect = JsonConvert.SerializeObject(returnError); ;
                return returnContect;
            }
            else
            {
                returnContect.Status = 200;
            }

            // Tokens
            response.accessToken = UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid());
            response.selectedProfile.accessToken = response.accessToken;

            if (request.clientToken == null)
            {
                response.clientToken = UuidWorker.GenUuid();
            }
            else
            {
                response.clientToken = request.clientToken;
                response.selectedProfile.clientToken = request.clientToken;
            }

            // Profiles
            response.selectedProfile.availableProfiles = new string[] { "Romonov" };

            // Users
            if (request.requestUser)
            {
                User user = new User();
                Profiles profiles = new Profiles();
                profiles.id = "romonov";
                Properties properties = new Properties();
                properties.name = "preferredLanguage";
                properties.value = "zh_cn";
                profiles.properties = new Properties[] { properties };
                user.profiles = new Profiles[] { profiles };
                response.user = user;
            }

            returnContect.Contect = JsonConvert.SerializeObject(response);

            return returnContect;
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
            public string[] availableProfiles;
            public SelectedProfile selectedProfile;
            public User? user;
        }

        struct SelectedProfile
        {
            public string accessToken;
            public string clientToken;
            public string[] availableProfiles;
        }

        struct User
        {
            public Profiles[] profiles;
        }

        struct Profiles
        {
            public string id;
            public Properties[] properties;
        }

        struct Properties
        {
            public string name;
            public string value;
        }
    }
}
