using Newtonsoft.Json;
using System;
using static Mimir.Common.Router;

namespace Mimir.Response.Exceptions
{
    class UsernameAlreadyUsed
    {
        public static Tuple<int, string> GetResponse()
        {
            ReturnError returnError = new ReturnError();
            returnError.error = "UsernameAlreadyUsed";
            returnError.errorMessage = "Your username is already used by another account, please change your username or retrieve your password.";
            returnError.cause = "Username already used!";

            return new Tuple<int, string>(403, JsonConvert.SerializeObject(returnError));
        }
    }
}
