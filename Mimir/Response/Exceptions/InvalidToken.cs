using Newtonsoft.Json;
using System;

namespace Mimir.Response.Exceptions
{
    class InvalidToken
    {
        public static ValueTuple<int, string, string> GetResponse()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid token.";
            result.cause = "Invalid token!";

            return (403, "text/plain", JsonConvert.SerializeObject(result));
        }

        public struct Result
        {
            public string error;
            public string errorMessage;
            public string cause;
        }
    }
}
