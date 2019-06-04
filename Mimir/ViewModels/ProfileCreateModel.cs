using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.ViewModels
{
    public class ProfileCreateModel
    {
        [Required(ErrorMessage = "角色名不能为空！")]
        [Remote(action: "VerifyName", controller: "Profile", ErrorMessage = "角色名已存在！")]
        [MaxLength(20, ErrorMessage = "角色名太长了！")]
        [MinLength(6, ErrorMessage = "角色名太短了！")]
        public string Name { get; set; }
    }
}
