using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;
using Newtonsoft.Json;
using NLog;

namespace Mimir.Controllers.Yggdrasil
{
    public class ApiController : ControllerBase
    {
        private MimirContext db = null;
        private ILogger log = null;

        public ApiController(MimirContext context)
        {
            db = context;
            log = LogManager.GetLogger("Yggdrasil");
        }

        [HttpGet]
        public JsonResult Index()
        {
            GetIndexResponse response = new GetIndexResponse();

            response.meta.serverName = Program.ServerName;
            response.meta.implementationName = "Mimir";
            response.meta.implementationVersion = Program.Version;
            response.skinDomains = Program.SkinDomains;
            response.signaturePublickey = $"-----BEGIN PUBLIC KEY-----\n{Program.PublicKey}\n-----END PUBLIC KEY-----\n";

            return new JsonResult(response);
        }

        [HttpPost]
        public JsonResult Profiles([FromBody] string[] request)
        {
            if (request.Length > Program.MaxProfileCountPerQuery)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]IP {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} tried to query profiles over the limit.");
                return new JsonResult(StatusCode((int)HttpStatusCode.Forbidden));
            }

            var profiles = new List<ProfileWorker.Profile>();
            foreach (var item in request)
            {
                var result = ProfileWorker.GetProfile(db, item, HttpContext);
                if (result != null)
                {
                    profiles.Add(result.Value);
                }
            }

            log.Info($"[ID: {HttpContext.Connection.Id}]IP {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} queried profiles.");
            return new JsonResult(profiles);
        }

        public struct GetIndexResponse
        {
            public GetIndexMeta meta;
            public string[] skinDomains;
            public string signaturePublickey;
        }

        public struct GetIndexMeta
        {
            public string serverName;
            public string implementationName;
            public string implementationVersion;
        }
    }
}