using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Newtonsoft.Json;

namespace Mimir.Controllers
{
    [Route("api/")]
    public class ApiController : ControllerBase
    {
        private MimirContext db = null;

        public ApiController(MimirContext context)
        {
            db = context;
        }

        [HttpGet]
        public ActionResult<string> Index()
        {
            GetIndexResponse response = new GetIndexResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = "Mimir";
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = new string[] { $"{Program.ServerDomain}" };
            response.signaturePublickey = $"-----BEGIN PUBLIC KEY-----\n{Program.PublicKey}\n-----END PUBLIC KEY-----\n";

            return JsonConvert.SerializeObject(response);
        }

        [Route("api/profiles/minecraft")]
        public ActionResult<string> Profiles()
        {
            return "";
        }

        struct GetIndexResponse
        {
            public GetIndexMeta meta;
            public string[] skinDomains;
            public string signaturePublickey;
        }

        struct GetIndexMeta
        {
            public string serverName;
            public string implementationName;
            public string implementationVersion;
        }
    }
}