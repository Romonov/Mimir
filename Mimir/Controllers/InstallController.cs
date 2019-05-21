using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;

namespace Mimir.Controllers
{
    public class InstallController : Controller
    {
        private MimirContext db = null;

        public InstallController(MimirContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (bool.Parse(db.Options.Find("IsInstalled").Value))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Install()
        {
            if (bool.Parse(db.Options.Find("IsInstalled").Value))
            {
                return NotFound();
            }

            SignatureWorker.GenKey(db);

            return View("Successful");
        }
    }
}