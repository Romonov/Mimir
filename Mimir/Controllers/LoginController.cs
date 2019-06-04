using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.ViewModels;
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
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "登录";
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromForm] LoginModel model)
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "登录";
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
                return RedirectToAction(actionName: "Index", controllerName: "User");
            }
            else
            {
                return View(model);
            }
        }
    }
}