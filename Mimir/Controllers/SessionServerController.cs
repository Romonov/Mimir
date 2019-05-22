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

namespace Mimir.Controllers
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
        public ActionResult<string> Join([FromBody] PostJoinRequest request)
        {
            // Check token.
            var tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.Status == 2 select t;
            if (tokens.Count() != 1)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidToken());
            }
            var token = tokens.First();

            // Check profile.
            var profiles = from p in db.Profiles where p.Uuid == request.selectedProfile select p;
            if (profiles.Count() != 1)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidToken());
            }
            var profile = profiles.First();
            if (token.BindProfileId != profile.Id)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidToken());
            }

            db.Sessions.Add(new Sessions()
            {
                AccessToken = request.accessToken,
                ServerId = request.serverId,
                ExpireTime = TimeWorker.GetJavaTimeStamp(Program.SessionsExpireSeconds),
                ClientIp = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString()
            });
            db.SaveChanges();

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("{username}/{serverId}/{ip}")]
        public ActionResult<string> HasJoined(string username, string serverId, string ip)
        {
            var profiles = from p in db.Profiles where p.Name == username select p;
            if (profiles.Count() != 1)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            var profile = profiles.First();

            var tokens = from t in db.Tokens where t.BindProfileId == profile.Id && t.Status == 2 select t;
            if (tokens.Count() != 1)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            var token = tokens.First();

            var time = long.Parse(TimeWorker.GetJavaTimeStamp());
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
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            var result = ProfileWorker.GetProfile(db, profile.Name, true, false);
            if (result != null)
            {
                return result;
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }

        [HttpGet("unsigned")]
        public ActionResult<string> Profile(string uuid, string unsigned)
        {
            if (bool.TryParse(unsigned, out var isUnsigned) && Guid.TryParse(uuid, out var guid))
            {
                return ProfileWorker.GetProfile(db, guid, true, isUnsigned);
            }
            else
            {
                return NotFound();
            }
        }

        public struct PostJoinRequest
        {
            public string accessToken;
            public string selectedProfile;
            public string serverId;
        }
    }
}