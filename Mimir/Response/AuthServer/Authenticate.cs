using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.AuthServer
{
    public class Authenticate
    {
        public static string OnPost(string PostData)
        {
            JsonConvert.DeserializeObject<Request>(PostData);

            return "";
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
    }
}
