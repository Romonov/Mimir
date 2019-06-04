using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;
using Mimir.ViewModels;

namespace Mimir.Controllers
{
    public class ProfileController : AuthControllerBase
    {
        private MimirContext db = null;

        public ProfileController(MimirContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色列表";
            var user = ByteConverter.ToObject<Users>(HttpContext.Session.Get("User"));
            var profiles = from p in db.Profiles where p.Uid == user.Id select p;
            var profilesList = new List<Profiles>();
            foreach (var item in profiles)
            {
                profilesList.Add(item);
            }
            var model = new ProfileIndexModel()
            {
                Profiles = profilesList
            };
            return View(model);
        }

        public IActionResult Create()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色创建";
            return View();
        }

        [HttpPost]
        public IActionResult Create([FromForm] ProfileCreateModel model)
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色创建";

            if (ModelState.IsValid)
            {
                var user = ByteConverter.ToObject<Users>(HttpContext.Session.Get("User"));
                var profiles = from p in db.Profiles where p.Uid == user.Id select p;
                if (profiles.Count() >= Program.MaxProfileCountPerUser)
                {
                    ModelState.AddModelError(string.Empty, "创建角色失败，角色数量已经达到上限！");
                    return View(model);
                }
                else
                {
                    db.Profiles.Add(new Profiles()
                    {
                        Name = model.Name,
                        Uuid = UuidWorker.GetUuid(),
                        Uid = user.Id,
                        SkinModel = 0,
                        SkinUrl = string.Empty,
                        CapeUrl = string.Empty,
                        IsSelected = 0
                    });
                    db.SaveChanges();
                    ViewData["Result"] = "创建角色成功！";
                    return View("Result");
                }
            }
            else
            {
                return View(model);
            }
        }

        public IActionResult Manage(string name)
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色管理";
            return View();
        }

        public IActionResult Bind()
        {
            ViewData["ServerName"] = Program.ServerName;
            ViewData["Title"] = "角色迁移";
            return View();
        }

        public IActionResult VerifyName(string name)
        {
            var profiles = from p in db.Profiles where p.Name == name select p;
            if (profiles.Count() >= 1)
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