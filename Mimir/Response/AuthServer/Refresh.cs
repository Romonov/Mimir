using Mimir.Response.Exceptions;
using Mimir.Common.SQL;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Mimir.Response.AuthServer
{
    public class Refresh
    {
        public static Tuple<int, string, string> OnPost(string PostData)
        {
            // Post /authserver/refresh
            Response response = new Response();

            Request request = JsonConvert.DeserializeObject<Request>(PostData);

            // Tokens
            DataSet dataSetToken = SqlProxy.Query("SELECT * FROM `tokens`");

            DataSet dataSetUser = SqlProxy.Query("SELECT * FROM `users`");

            DataRow dataRowToken = null;
            DataRow dataRowUser = null;

            foreach (DataRow dataRow in dataSetToken.Tables[0].Rows)
            {
                if (dataRow["AccessToken"].ToString() == request.accessToken)
                {
                    if (request.clientToken != null)
                    {
                        if (dataRow["ClientToken"].ToString() != request.clientToken)
                        {
                            return InvalidToken.GetResponse();
                        }
                    }

                    dataRowToken = dataRow;
                }
                else
                {
                    return InvalidToken.GetResponse();
                }
            }

            foreach (DataRow dataRow in dataSetUser.Tables[0].Rows)
            {
                if (dataRow["Username"].ToString() == dataRowToken["BindUser"].ToString())
                {
                    dataRowUser = dataRow;
                }
            }

            response.accessToken = dataRowToken["AccessToken"].ToString();
            response.clientToken = dataRowToken["ClientToken"].ToString();

            // Profiles

            if (request.selectedProfile.HasValue)
            {
                response.selectedProfile = request.selectedProfile;
            }
            else
            {
                SelectedProfile playerProfile = new SelectedProfile();
                DataSet dataSetProfiles = SqlProxy.Query("SELECT * FROM `profiles`;");

                foreach (DataRow dataRow in dataSetProfiles.Tables[0].Rows)
                {
                    if (dataRow["UserID"].ToString() == dataRowUser["Username"].ToString() && dataRow["IsSelected"].ToString() == "True")
                    {
                        playerProfile.id = dataRow["UnsignedUUID"].ToString();
                        playerProfile.name = dataRow["Name"].ToString();
                    }
                }

                response.selectedProfile = playerProfile;
            }


            // Users

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

            return new Tuple<int, string, string>(200, "text/plain", JsonConvert.SerializeObject(response));
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
