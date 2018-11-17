using Newtonsoft.Json;
using System;
using static Mimir.Common.Router;

namespace Mimir.Response.Exceptions
{
    class EmailAlreadyUsed
    {
        public static Tuple<int, string> GetResponse()
        {
            ReturnError returnError = new ReturnError();
            returnError.error = "EmailAlreadyUsed";
            returnError.errorMessage = "Your email is already used by another account, please change your email or retrieve your password.";
            returnError.cause = "Email already used!";

            return new Tuple<int, string>(403, JsonConvert.SerializeObject(returnError));
        }
    }
}
