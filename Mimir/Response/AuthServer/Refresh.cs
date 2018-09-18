using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.AuthServer
{
    public class Refresh
    {
        public static string OnPost(string PostData)
        {
            JsonConvert.DeserializeObject<Request>(PostData);

            return "";
        }

        struct Request
        {
            public string accessToken;
            public string clientToken;
            public bool requestUser;
            public SelectedProfile selectedProfile;
        }
        
        struct SelectedProfile
        {

        }

        struct Response
        {
            public string accessToken;
            public string clientToken;
            public SelectedProfile selectedProfile;
            public User? user;
        }

        struct User
        {

        }
    }

}
