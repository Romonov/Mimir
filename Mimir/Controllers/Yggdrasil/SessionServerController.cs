using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;
using NLog;

namespace Mimir.Controllers.Yggdrasil
{
    public class SessionServerController : ControllerBase
    {
        private MimirContext db = null;
        private ILogger log = null;

        public SessionServerController(MimirContext context)
        {
            db = context;
            log = LogManager.GetLogger("SessionServer");
        }

        [HttpPost]
        public JsonResult Join([FromBody] PostJoinRequest request)
        {
            // Check token.
            var tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.Status == 2 select t;
            if (tokens.Count() != 1)
            {
                return new JsonResult(ExceptionWorker.InvalidToken())
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
            var token = tokens.First();

            // Check profile.
            var profiles = from p in db.Profiles where p.Uuid == request.selectedProfile select p;
            if (profiles.Count() != 1)
            {
                return new JsonResult(ExceptionWorker.InvalidToken())
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
            var profile = profiles.First();
            if (token.BindProfileId != profile.Id)
            {
                return new JsonResult(ExceptionWorker.InvalidToken())
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }

            db.Sessions.Add(new Sessions()
            {
                AccessToken = request.accessToken,
                ServerId = request.serverId,
                ExpireTime = TimeWorker.GetTimeStamp10(Program.SessionsExpireSeconds),
                ClientIp = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
            });
            db.SaveChanges();

            // Clean expired sessions.
            var time = long.Parse(TimeWorker.GetTimeStamp10());
            var sessions = from s in db.Sessions where long.Parse(s.ExpireTime) < time select s;
            foreach (var item in sessions)
            {
                db.Sessions.Remove(item);
            }
            db.SaveChanges();

            log.Info($"[ID: {HttpContext.Connection.Id}]Player {profile.Name} with IP {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} tried to login server.");

            return new JsonResult(null)
            {
                StatusCode = (int)HttpStatusCode.NoContent
            };
        }

        [HttpGet]
        public JsonResult HasJoined()
        {
            string username = Request.Query["username"];
            string serverId = Request.Query["serverId"];
            string ip = Request.Query["ip"];

            if (username == null || username == string.Empty || serverId == null || serverId == string.Empty)
            {
                return new JsonResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NoContent
                };
            }

            var profiles = from p in db.Profiles where p.Name == username select p;
            if (profiles.Count() != 1)
            {
                return new JsonResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NoContent
                };
            }
            var profile = profiles.First();

            var tokens = from t in db.Tokens where t.BindProfileId == profile.Id && t.Status == 2 select t;
            if (tokens.Count() != 1)
            {
                return new JsonResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NoContent
                };
            }
            var token = tokens.First();

            var time = long.Parse(TimeWorker.GetTimeStamp10());
            IQueryable<Sessions> sessions = null;
            if (ip != null && ip != string.Empty)
            {
                sessions = from s in db.Sessions where long.Parse(s.ExpireTime) >= time && s.AccessToken == token.AccessToken && s.ServerId == serverId && s.ClientIp == ip select s;
            }
            else
            {
                sessions = from s in db.Sessions where long.Parse(s.ExpireTime) >= time && s.AccessToken == token.AccessToken && s.ServerId == serverId select s;
            }
            if (sessions.Count() != 1)
            {
                return new JsonResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NoContent
                };
            }

            var result = ProfileWorker.GetProfile(db, profile.Name, HttpContext, true, false);
            if (result != null)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]Player {profile.Name} login successfully.");
                return new JsonResult(result.Value);
            }
            else
            {
                return new JsonResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NoContent
                };
            }
        }

        [HttpGet]
        public JsonResult Profile(string uuid)
        {
            log.Info($"[ID: {HttpContext.Connection.Id}]IP {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} requested profile with uuid {uuid}.");

            string unsigned = Request.Query["unsigned"];

            if (unsigned == null || unsigned == string.Empty)
            {
                unsigned = true.ToString();
            }

            if (Guid.TryParse(uuid, out var guid) && bool.TryParse(unsigned, out var isUnsigned))
            {
                var result = ProfileWorker.GetProfile(db, guid, HttpContext, true, isUnsigned);
                if (result != null)
                {
                    return new JsonResult(result);
                }
            }

            return new JsonResult(null)
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        public struct PostJoinRequest
        {
            public string accessToken;
            public string selectedProfile;
            public string serverId;
        }
    }
}