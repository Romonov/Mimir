using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mimir.Controllers
{
    public class IndexControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (Program.IsEnableLandingPage)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                if (context.HttpContext.Session.TryGetValue("User", out _))
                {
                    context.Result = new RedirectResult("/User");
                }
                else
                {
                    context.Result = new RedirectResult("/Login");
                }
            }
        }
    }
}
