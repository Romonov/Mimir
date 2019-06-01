using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;

namespace Mimir.Controllers
{
    public class LoginController : Controller
    {
        private MimirContext db = null;

        public LoginController(MimirContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] LoginModel model)
        {
            ViewData["ServerName"] = Program.ServerName;
            if (ModelState.IsValid)
            {
                var users = from u in db.Users where u.Username == model.Username select u;
                if (users.Count() != 1)
                {
                    ModelState.AddModelError("", "用户名或密码错误！");
                    return View(model);
                }
                var user = users.First();
                var password = HashWorker.HashPassword(model.Password, user.CreateTime);
                if (password != user.Password)
                {
                    ModelState.AddModelError("", "用户名或密码错误！");
                    return View(model);
                }
                HttpContext.Session.Set("User", ByteConverter.ToBytes(user));
                return View("Index");
            }
            else
            {
                return View(model);
            }
        }
    }
}