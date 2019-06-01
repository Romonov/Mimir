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
        [Compare("RepeatPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "重复密码不能为空！")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string RepeatPassword { get; set; }

    }
}
