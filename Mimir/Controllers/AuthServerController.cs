using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mimir.Models;
using Mimir.Util;
using Newtonsoft.Json;
using NLog;

namespace Mimir.Controllers
{
    public class AuthServerController : ControllerBase
    {
        private MimirContext db = null;
        private ILogger log = null;

        public AuthServerController(MimirContext context)
        {
            db = context;
            log = LogManager.GetLogger("AuthServer");
        }

        [HttpPost]
        public ActionResult<string> Authenticate([FromBody] PostAuthrnticateRequest request)
        {
            log.Info($"[ID: {HttpContext.Connection.Id}]Got login request from {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort} with user {request.username}.");

            var users = from u in db.Users where u.Email == request.username select u;
            if (users.Count() != 1)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]Request user is not exists.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvaildUsername());
            }

            // Cooldown check.
            var user = users.First();
            var time = TimeWorker.GetJavaTimeStamp();
            var cooldowns = from c in db.Cooldown where c.Uid == user.Id select c;
            if (cooldowns.Count() != 1)
            {
                db.Cooldown.Add(new Cooldown()
                {
                    Uid = user.Id,
                    TryTimes = 0,
                    LastTryTime = time,
                    LastLoginTime = user.CreateTime,
                    CooldownLevel = 0,
                    CooldownTime = time
                });
                db.SaveChanges();
            }

            cooldowns = from c in db.Cooldown where c.Uid == user.Id select c;
            var cooldown = cooldowns.First();
            if (Convert.ToDecimal(cooldown.CooldownTime) > Convert.ToDecimal(time))
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]User {request.username} already in cooldown.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.TooManyTryTimes());
            }
            else
            {
                if (cooldown.TryTimes >= Program.SecurityLoginTryTimes)
                {
                    cooldown.CooldownLevel++;
                    cooldown.CooldownTime = time + cooldown.CooldownLevel * cooldown.CooldownLevel * 5 * 60;
                    db.SaveChanges();
                    log.Info($"[ID: {HttpContext.Connection.Id}]User {request.username} got into cooldown.");
                    return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.TooManyTryTimes());
                }
                cooldown.LastTryTime = time;
                cooldown.TryTimes++;
                db.SaveChanges();
            }

            // Password check.
            var salt = user.CreateTime;
            var passwordHashed = HashWorker.HashPassword(request.password, salt);

            if (user.Password != passwordHashed)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]IP address {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort} try to login with user {request.username} but wrong password.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvaildPassword());
            }

            // Update cooldown.
            cooldown.LastLoginTime = time;
            cooldown.TryTimes = 0;
            db.SaveChanges();
            log.Info($"[ID: {HttpContext.Connection.Id}]Cooldown of user {request.username} has reseted.");

            // Hand token out and select profile.
            var accessToken = UuidWorker.GetUuid();
            var clientToken = string.Empty;
            if (request.clientToken != null)
            {
                clientToken = request.clientToken;
            }
            else
            {
                clientToken = UuidWorker.GetUuid();
            }

            Tokens token = new Tokens()
            {
                AccessToken = accessToken,
                ClientToken = clientToken,
                CreateTime = time,
                Status = 2
            };

            PostAuthrnticateResponse response = new PostAuthrnticateResponse();
            var profiles = from p in db.Profiles where p.Uid == user.Id select p;

            List<Profile> availableProfiles = new List<Profile>();
            foreach (var p in profiles)
            {
                var playerProfile = new Profile();
                playerProfile.id = p.Uuid;
                playerProfile.name = p.Name;

                if (profiles.Count() > 1)
                {
                    if (p.IsSelected == 1)
                    {
                        response.selectedProfile = playerProfile;
                        token.BindProfileId = p.Id;
                        log.Info($"[ID: {HttpContext.Connection.Id}]User {request.username} has logged and binded profile {playerProfile.name}.");
                    }
                }
                else if (profiles.Count() == 1)
                {
                    response.selectedProfile = playerProfile;
                    token.BindProfileId = p.Id;
                    p.IsSelected = 1;
                    log.Info($"[ID: {HttpContext.Connection.Id}]User {request.username} has logged and binded profile {playerProfile.name}.");
                }
                else
                {
                    log.Info($"[ID: {HttpContext.Connection.Id}]User {request.username} has logged but not bind any profile.");
                }

                availableProfiles.Add(playerProfile);
            }

            db.Tokens.Add(token);
            db.SaveChanges();

            // Build response
            response.accessToken = accessToken;
            response.clientToken = clientToken;

            var availableProfilesFinal = availableProfiles.ToArray();
            response.availableProfiles = availableProfilesFinal;

            if (request.requestUser)
            {
                var properties = new Properties()
                {
                    name = "preferredLanguage",
                    value = user.PreferredLanguage
                };
                response.user = new User()
                {
                    id = user.Username
                };
            }

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public ActionResult<string> Refresh([FromBody] PostRefreshRequest request)
        {
            // Check token
            IQueryable<Tokens> tokens = null;
            if (request.clientToken != null)
            {
                tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.ClientToken == request.clientToken && t.Status >= 1 select t;
            }
            else
            {
                tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.Status >= 1 select t;
            }

            if (tokens.Count() != 1)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvaildToken());
            }

            // Invaild token.
            var token = tokens.First();
            token.Status = 0;
            db.SaveChanges();

            // Check others invaild tokens.
            var time = long.Parse(TimeWorker.GetJavaTimeStamp());
            var invaildTokens = from t in db.Tokens where (long.Parse(t.CreateTime) + Program.TokensExpireDaysLimit * 24 * 60 * 60) <= time select t;
            foreach (var item in invaildTokens)
            {
                item.Status = 1;
            }
            db.SaveChanges();

            // Delete invaild tokens.
            invaildTokens = from t in db.Tokens where t.Status == 0 select t;
            foreach (var item in invaildTokens)
            {
                db.Tokens.Remove(item);
            }
            db.SaveChanges();

            // Bind profile.
            Tokens tokenNew = new Tokens();
            if (request.selectedProfile != null)
            {
                var profiles = from p in db.Profiles where p.Uuid == request.selectedProfile.Value.id select p;
                if (profiles.Count() == 1)
                {
                    var profile = profiles.First();
                    profile.IsSelected = 1;
                    tokenNew.BindProfileId = profile.Id;
                }
            }

            // Check if token reach the limit.


            // Build response and hand the new token out.
            PostRefreshResponse response = new PostRefreshResponse();

            response.accessToken = UuidWorker.GetUuid();
            if (request.clientToken != null)
            {
                response.clientToken = tokenNew.ClientToken = request.clientToken;
            }
            else
            {
                response.clientToken = tokenNew.ClientToken = UuidWorker.GetUuid();
            }
            tokenNew.CreateTime = time.ToString();
            tokenNew.Status = 2;
            db.Tokens.Add(tokenNew);
            db.SaveChanges();

            if (request.selectedProfile != null)
            {
                
            }
            
            /*
            if (request.requestUser)
            {
                var users = from u in db.Users where u.Id == uid select u;
                var user = users.First();

                var properties = new Properties()
                {
                    name = "preferredLanguage",
                    value = user.PreferredLanguage
                };
                response.user = new User()
                {
                    id = user.Username
                };
            }
            */

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public ActionResult<string> Vaildate()
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<string> Invaildate()
        {
            return NotFound();
        }

        [HttpPost]
        public ActionResult<string> Signout()
        {
            return NotFound();
        }

        public struct Profile
        {
            public string id;
            public string name;
            public Properties[] properties;
        }
        public new struct User
        {
            public string id;
            public Properties[] properties;
        }
        public struct Properties
        {
            public string name;
            public string value;
            public string signature;
        }

        public struct PostAuthrnticateRequest
        {
            public string username;
            public string password;
            public string clientToken;
            public bool requestUser;
            public PostAuthrnticateRequestAgent agent;
        }
        public struct PostAuthrnticateRequestAgent
        {
            public string name;
            public int version;
        }

        public struct PostAuthrnticateResponse
        {
            public string accessToken;
            public string clientToken;
            public Profile[] availableProfiles;
            public Profile? selectedProfile;
            public User? user;
        }
        
        public struct PostRefreshRequest
        {
            public string accessToken;
            public string clientToken;
            public bool requestUser;
            public Profile? selectedProfile;
        }

        public struct PostRefreshResponse
        {
            public string accessToken;
            public string clientToken;
            public Profile? selectedProfile;
            public User? user;
        }
    }
}