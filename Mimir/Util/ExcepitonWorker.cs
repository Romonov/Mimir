using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.Util
{
    /// <summary>
    /// 业务异常工具类
    /// Todo: I18n
    /// </summary>
    public static class ExcepitonWorker
    {
        public static string InvaildToken()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid token.";
            result.cause = "Invalid token.";
            return JsonConvert.SerializeObject(result);
        }

        public static string InvaildPassword()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Invalid password.";
            return JsonConvert.SerializeObject(result);
        }

        public static string InvaildUsername()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Invalid username.";
            return JsonConvert.SerializeObject(result);
        }

        public static string BadProfile()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid token.";
            result.cause = "Bad profile.";
            return JsonConvert.SerializeObject(result);
        }

        public static string TooManyTryTimes()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Too many try times.";
            return JsonConvert.SerializeObject(result);
        }

        public static string ProfileNotAllowed()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Profile not belong to this user.";
            result.cause = "Profile not belong to this user.";
            return JsonConvert.SerializeObject(result);
        }

        public static string AlreadyBind()
        {
            Result result = new Result();
            result.error = "IllegalArgumentException";
            result.errorMessage = "Access token already has a profile assigned.";
            result.cause = "This token already has a profile.";
            return JsonConvert.SerializeObject(result);
        }

        public struct Result
        {
            public string error;
            public string errorMessage;
            public string cause;
        }
    }
}
