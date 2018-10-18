using Newtonsoft.Json;
using System;
using static Mimir.Common.Router;

namespace Mimir.Response.Exceptions
{
    class InvalidPassword
    {
        public static Tuple<int, string> GetResponse()
        {
            ReturnError returnError = new ReturnError();
            returnError.error = "ForbiddenOperationException";
            returnError.errorMessage = "Invalid credentials. Invalid username or password.";

            return new Tuple<int, string>(403, JsonConvert.SerializeObject(returnError));
        }
    }
}
