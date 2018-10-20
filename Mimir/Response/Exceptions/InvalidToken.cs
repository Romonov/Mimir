using Newtonsoft.Json;
using System;
using static Mimir.Common.Router;

namespace Mimir.Response.Exceptions
{
    class InvalidToken
    {
        public static Tuple<int, string> GetResponse()
        {
            ReturnError returnError = new ReturnError();
            returnError.error = "ForbiddenOperationException";
            returnError.errorMessage = "Invalid token.";
            returnError.cause = "Invalid token!";

            return new Tuple<int, string>(403, JsonConvert.SerializeObject(returnError));
        }
    }
}
