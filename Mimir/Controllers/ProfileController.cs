using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mimir.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色列表";
            return View();
        }

        public IActionResult Create()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "创建角色";
            return View();
        }
    }
}