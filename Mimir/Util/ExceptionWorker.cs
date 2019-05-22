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
    public static class ExceptionWorker
    {
        public static Result InvalidToken()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid token.";
            result.cause = "Invalid token.";
            return result;
        }

        public static Result InvalidPassword()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Invalid password.";
            return result;
        }

        public static Result InvalidUsername()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Invalid username.";
            return result;
        }

        public static Result BadProfile()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid token.";
            result.cause = "Bad profile.";
            return result;
        }

        public static Result TooManyTryTimes()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Invalid credentials. Invalid username or password.";
            result.cause = "Too many try times.";
            return result;
        }

        public static Result ProfileNotAllowed()
        {
            Result result = new Result();
            result.error = "ForbiddenOperationException";
            result.errorMessage = "Profile not belong to this user.";
            result.cause = "Profile not belong to this user.";
            return result;
        }

        public static Result AlreadyBind()
        {
            Result result = new Result();
            result.error = "IllegalArgumentException";
            result.errorMessage = "Access token already has a profile assigned.";
            result.cause = "This token already has a profile.";
            return result;
        }

        public struct Result
        {
            public string error;
            public string errorMessage;
            public string cause;
        }
    }
}
