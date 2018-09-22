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

            DataSet dataSet = SqlProxy.Querier($"SELECT users.Email FROM users WHERE users.Email = {request.username}");
            DataRow[] dataRows = dataSet.Tables[0].Select();
            if (request.password != dataRows[0]["Password"].ToString())
            {
                returnContect.Status = 403;
                return returnContect;
            }
            else
            {
                returnContect.Status = 200;
            }

            response.accessToken = UuidWorker.ToUnsignedUuid(UuidWorker.GenUuid());

            if(request.clientToken == null)
            {
                response.clientToken = UuidWorker.GenUuid();
            }

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
            public string accessToken;
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
