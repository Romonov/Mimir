using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "用户名不能为空！")]
        public string Username { set; get; }

        [Required(ErrorMessage = "密码不能为空！")]
        [DataType(DataType.Password)]
        [MaxLength(30, ErrorMessage = "你的密码太长了！")]
        [MinLength(6, ErrorMessage = "你的密码太短了！")]
        public string Password { get; set; }

        [Required(ErrorMessage = "重复密码不能为空！")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "邮箱不能为空！")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "邮箱验证码不能为空！")]
        public string VerificationCode { get; set; }
    }
}
