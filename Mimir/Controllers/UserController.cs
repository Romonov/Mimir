using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;

namespace Mimir.Controllers
{
    public class UserController : AuthControllerBase
    {
        private MimirContext db = null;

        public UserController(MimirContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "用户";
            if (Program.IsHttps)
            {
                ViewData["HttpOrHttps"] = "https";
            }
            else
            {
                ViewData["HttpOrHttps"] = "http";
            }
            ViewData["ServerDomain"] = Program.ServerDomain;
            return View();
        }
    }
}