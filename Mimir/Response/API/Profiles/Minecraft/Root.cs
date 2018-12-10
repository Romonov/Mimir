using Mimir.Response.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.API.Profiles.Minecraft
{
    class Root
    {
        public static Tuple<int, string, string> OnPost(string postData)
        {
            // Post /api/profiles/minecraft

            Request request = JsonConvert.DeserializeObject<Request>(postData);

            string profileInfo = "";

            for (int i = 0; i < request.name.Length; i++)
            {
                profileInfo += GetProfile.Get(request.name[i]);

                if (i < request.name.Length - 1)
                {
                    profileInfo += ",";
                }
            }

            return new Tuple<int, string, string>(200, "text/plain", $"[{profileInfo}]");
        }

        struct Request
        {
            public string[] name;
        }
    }
}
