using Newtonsoft.Json;
using System;
using static Mimir.Common.Router;

namespace Mimir.Response.Exceptions
{
    class InvalidToken
    {
        public static Tuple<int, string, string> GetResponse()
        {
            ReturnError returnError = new ReturnError();
            returnError.error = "ForbiddenOperationException";
            returnError.errorMessage = "Invalid token.";
            returnError.cause = "Invalid token!";

            return new Tuple<int, string, string>(403, "text/plain", JsonConvert.SerializeObject(returnError));
        }
    }
}
