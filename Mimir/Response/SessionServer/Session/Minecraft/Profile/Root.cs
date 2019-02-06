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
        public static ValueTuple<int, string, string> OnGet(Dictionary<string, string> getData, Guid uuid)
        {
            // Get /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
            string profileInfo = "";

            if (getData != null && getData.ContainsKey("unsigned"))
            {
                profileInfo = GetProfile.Get(uuid, true, BoolParse(getData["unsigned"]));
            }
            else
            {
                profileInfo = GetProfile.Get(uuid, true);
            }

            if (profileInfo == "")
            {
                return (204, "text/plain", "");
            }

            return (200, "text/plain", profileInfo);
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
