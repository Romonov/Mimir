using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mimir.Controllers
{
    public class NeedLoginControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            byte[] result;
            filterContext.HttpContext.Session.TryGetValue("User", out result);
            if (result == null)
            {
                filterContext.Result = new RedirectResult("/login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
