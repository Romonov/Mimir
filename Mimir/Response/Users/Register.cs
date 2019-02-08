using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mimir.Response.Users
{
    class Register
    {
        public static ValueTuple<int, string, string> OnGet()
        {
            return (200, "text/html", 
                $"<form action=\"register\" method=\"post\">" +
                $"<p>{Program.ServerName} Register</p>" +
                $"<p>Username *: <input type=\"text\" name=\"username\" required /></p>" +
                $"<p>Password *: <input type=\"password\" name=\"password\" required /></p>" +
                $"<p>Email *: <input type=\"text\" name=\"email\" required /></p>" +
                $"<p>Nickname *: <input type=\"text\" name=\"nickname\" required /></p>" +
                $"<p>Profile Name *: <input type=\"text\" name=\"profile\" required /></p>" +
                $"<input type=\"submit\" value=\"Submit\" />" +
                $"</form>"
            );
        }

        public static ValueTuple<int, string, string> OnPost(string postData)
        {
            if (Program.UserRegisterTimes > Program.UserRegisterTimesPerMinute)
            {
                return (403, "text/plain", "Bad operation.");
            }

            string originData = HttpUtility.UrlDecode(postData, Encoding.Default);
            string[] splitedData = originData.Split('&');
            Dictionary<string, string> regData = new Dictionary<string, string>();
            foreach (string data in splitedData)
            {
                string[] tmp = data.Split('=');
                if (tmp.Length == 2)
                {
                    regData.Add(tmp[0], tmp[1]);
                }
            }

            if ((!regData.ContainsKey("username")) 
                || (!regData.ContainsKey("password")) 
                || (!regData.ContainsKey("email")) 
                || (!regData.ContainsKey("nickname")) 
                || (!regData.ContainsKey("profile")))
            {
                return (403, "text/plain", "Bad operation.");
            }
            


            return (200, "text/html", "Register successful!");
        }
    }
}
