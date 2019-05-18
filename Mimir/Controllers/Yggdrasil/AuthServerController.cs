using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mimir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthServerController : ControllerBase
    {
        public ActionResult Authenticate()
        {
            return NotFound();
        }

        public ActionResult Refresh()
        {
            return NotFound();
        }

        public ActionResult Vaildate()
        {
            return NotFound();
        }

        public ActionResult Invaildate()
        {
            return NotFound();
        }

        public ActionResult Signout()
        {
            return NotFound();
        }
    }
}