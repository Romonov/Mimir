﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Mimir.Controllers
{
    public class IndexController : IndexControllerBase
    {
        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "首页";
            return View();
        }
    }
}