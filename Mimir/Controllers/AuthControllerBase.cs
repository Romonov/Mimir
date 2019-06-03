using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mimir.Controllers
{
    public class AuthControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Session.TryGetValue("User", out _))
            {
                context.Result = new RedirectResult("/Login");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
