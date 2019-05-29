using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;

namespace Mimir.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            return View();
        }

        public IActionResult Register()
        {
            ViewData["ServerName"] = Program.ServerName;
            return View();
        }

        public IActionResult Login()
        {
            ViewData["ServerName"] = Program.ServerName;
            if (HttpContext.Request.Method == "GET")
            {
                return View();
            }
            else
            {
                return View("Index");
            }
        }
    }
}