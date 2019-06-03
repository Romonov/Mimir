using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mimir.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "注册";
            return View();
        }
    }
}