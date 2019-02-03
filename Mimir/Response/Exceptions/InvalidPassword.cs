using Newtonsoft.Json;
using System;

namespace Mimir.Response.Exceptions
{
    class InvalidPassword
    {
        public static ValueTuple<int, string, string> GetResponse()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Invalid password!";

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
