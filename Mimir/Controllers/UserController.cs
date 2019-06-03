using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;

namespace Mimir.Controllers
{
    public class UserController : Controller
    {
        private MimirContext db = null;

        public UserController(MimirContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "用户信息";
            return View();
        }
    }
}