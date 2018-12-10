using Mimir.Common;
using Mimir.Response.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Response.SessionServer.Session.Minecraft.Profile
{
    class Root
    {
        public static Tuple<int, string, string> OnGet(Dictionary<string, string> getData, Guid uuid)
        {
            // Get /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
            string profileInfo = "";

            if (getData.ContainsKey("unsigned"))
            {
                profileInfo = GetProfile.Get(uuid, BoolParse(getData["unsigned"]));
            }
            else
            {
                profileInfo = GetProfile.Get(uuid);
            }

            if (profileInfo == "")
            {
                return new Tuple<int, string, string>(204, "text/plain", "");
            }

            return new Tuple<int, string, string>(200, "text/plain", profileInfo);
        }

        private static bool BoolParse(string str)
        {
            if (str.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
