using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "用户名不能为空！")]
        [Remote(action: "VerifyUsername", controller: "Register", ErrorMessage = "用户名已存在！")]
        public string Username { set; get; }

        [Required(ErrorMessage = "密码不能为空！")]
        [DataType(DataType.Password)]
        [MaxLength(30, ErrorMessage = "你的密码太长了！")]
        [MinLength(6, ErrorMessage = "你的密码太短了！")]
        public string Password { get; set; }

        [Required(ErrorMessage = "重复密码不能为空！")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "两次输入的密码不匹配！")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "邮箱不能为空！")]
        [EmailAddress]
        [Remote(action: "VerifyEmail", controller: "Register", ErrorMessage = "邮箱已存在！")]
        public string Email { get; set; }

        [Required(ErrorMessage = "邮箱验证码不能为空！")]
        [MaxLength(6, ErrorMessage = "验证码太长了！")]
        [MinLength(6, ErrorMessage = "验证码太短了！")]
        public string VerificationCode { get; set; }
    }
}
