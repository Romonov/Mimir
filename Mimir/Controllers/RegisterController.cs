using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.ViewModels;
using Mimir.Util;

namespace Mimir.Controllers
{
    public class RegisterController : Controller
    {
        private MimirContext db = null;

        public RegisterController(MimirContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "注册";
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm] RegisterModel model)
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "注册";
            if (ModelState.IsValid)
            {
                if (model.VerificationCode == HttpContext.Session.GetString("VerificationCode"))
                {
                    var time = TimeWorker.GetTimeStamp10();
                    var password = HashWorker.HashPassword(model.Password, time);
                    db.Users.Add(new Users()
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = password,
                        PreferredLanguage = "zh_CN",
                        CreateIp = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(), 
                        CreateTime = time,
                        PermissionLevel = 0,
                        IsEmailVerified = 1
                    });
                    db.SaveChanges();
                }
                return RedirectToAction(actionName: "Index", controllerName: "User");
            }
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult SendEmail(string email)
        {
            VerificationCodeWorker.Send(HttpContext, email);
            return View("Index");
        }

        public IActionResult VerifyEmail(string email)
        {
            var users = from u in db.Users where u.Email == email select u;
            if (users.Count() >= 1)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        public IActionResult VerifyUsername(string username)
        {
            var users = from u in db.Users where u.Username == username select u;
            if (users.Count() >= 1)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }
    }
}