using Mimir.Response.Common;
using Mimir.Response.Exceptions;
using Newtonsoft.Json;
using System;

namespace Mimir.Response.API.Profiles.Minecraft
{
    class Root
    {
        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            // Post /api/profiles/minecraft

            Request request = JsonConvert.DeserializeObject<Request>(postData);

            if (request.name.Length > Program.UserMaxApiQuery)
            {
                return ForbiddenOperation.GetResponse();
            }

            string profileInfo = "";

            for (int i = 0; i < request.name.Length; i++)
            {
                profileInfo += GetProfile.Get(request.name[i]);

                if (i < request.name.Length - 1)
                {
                    profileInfo += ",";
                }
            }

            return (200, "text/plain", $"[{profileInfo}]");
        }

        struct Request
        {
            public string[] name;
        }
    }
}
