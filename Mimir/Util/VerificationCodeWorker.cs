using Microsoft.AspNetCore.Http;
using System;

namespace Mimir.Util
{
    public class VerificationCodeWorker
    {
        public static string Send(HttpContext context, string email)
        {
            var VerificationCode = new Random().Next(100000, 999999);
            context.Session.SetString("VerificationCode", VerificationCode.ToString());
            var result = MailWorker.Send(email, "请验证你的邮箱！", $"你的验证码是：{VerificationCode}");
            if (result == string.Empty)
            {
                return string.Empty;
            }
            else
            {
                return result;
            }
        }
    }
}
