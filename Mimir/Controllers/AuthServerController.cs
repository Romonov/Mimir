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
            log.Info($"[ID: {HttpContext.Connection.Id}]Got login request from {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} with user {request.username}.");

            // Check if user exists.
            var users = from u in db.Users where u.Email == request.username select u;
            if (users.Count() != 1)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]Request user is not exists.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidUsername());
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
                log.Info($"[ID: {HttpContext.Connection.Id}]User {user.Username} already in cooldown.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.TooManyTryTimes());
            }
            else
            {
                if (cooldown.TryTimes >= Program.SecurityLoginTryTimes)
                {
                    cooldown.CooldownLevel++;
                    cooldown.CooldownTime = time + cooldown.CooldownLevel * cooldown.CooldownLevel * 5 * 60;
                    db.SaveChanges();
                    log.Info($"[ID: {HttpContext.Connection.Id}]User {user.Username} got into cooldown.");
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
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidPassword());
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
            log.Info($"[ID: {HttpContext.Connection.Id}]{HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} tried to refresh token.");
            var isAlreadyBindProfile = false;

            // Check token.
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
                log.Info($"[ID: {HttpContext.Connection.Id}]Token invalid.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidToken());
            }

            // Invalid token.
            int? profileId = null;
            int? userId = null;
            var token = tokens.First();
            if (token.BindProfileId != null)
            {
                profileId = token.BindProfileId;
                isAlreadyBindProfile = true;
            }
            else
            {
                var profiles = from p in db.Profiles where p.Id == profileId select p;
                if (profiles.Count() == 1)
                {
                    userId = profiles.First().Uid;
                }
            }
            token.Status = 0;
            db.SaveChanges();
            log.Info($"[ID: {HttpContext.Connection.Id}]Access token {token.AccessToken} has invalided.");

            // Check others temp invalid tokens.
            var time = long.Parse(TimeWorker.GetJavaTimeStamp());
            var tempInvalidTokens = from t in db.Tokens where (long.Parse(t.CreateTime) + Program.TokensExpireDaysLimit * 24 * 60 * 60) <= time select t;
            foreach (var t in tempInvalidTokens)
            {
                t.Status = 1;
            }
            db.SaveChanges();

            // Delete invalid tokens.
            var invalidTokens = from t in db.Tokens where t.Status == 0 select t;
            foreach (var t in invalidTokens)
            {
                db.Tokens.Remove(t);
            }
            db.SaveChanges();

            // Bind profile.
            PostRefreshResponse response = new PostRefreshResponse();

            Tokens tokenNew = new Tokens();
            if (request.selectedProfile != null)
            {
                if (isAlreadyBindProfile)
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.AlreadyBind());
                }
                else
                {
                    var profiles = from p in db.Profiles where p.Uuid == request.selectedProfile.Value.id select p;
                    var profile = profiles.First();
                    if (profiles.Count() == 1)
                    {
                        profile.IsSelected = 1;
                        profileId = profile.Id;
                        tokenNew.BindProfileId = profile.Id;
                        response.selectedProfile = request.selectedProfile;
                    };
                    userId = profile.Uid;
                    profiles = from p in db.Profiles where p.Uid == profile.Uid select p;
                    foreach (var p in profiles)
                    {
                        if (p.Id != profileId && p.IsSelected == 1)
                        {
                            p.IsSelected = 0;
                        }
                    }
                    db.SaveChanges();
                    log.Info($"[ID: {HttpContext.Connection.Id}]Bind profile {profile.Name}.");
                }
            }

            // Check if token reach the limit.
            tokens = from t in db.Tokens where t.BindProfileId == profileId && t.Status == 1 select t;
            if (tokens.Count() > Program.MaxTokensPerProfile)
            {
                long createTime = long.MaxValue;
                foreach (var t in tokens)
                {
                    if (long.Parse(t.CreateTime) <= createTime)
                    {
                        createTime = long.Parse(t.CreateTime);
                    }
                }
                tokens = from t in db.Tokens where t.BindProfileId == profileId && t.CreateTime == createTime.ToString() select t;
                tokens.First().Status = 0;
                db.SaveChanges();
            }

            // Build response and hand the new token out.
            response.accessToken = tokenNew.AccessToken = UuidWorker.GetUuid();
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

            // User info.
            if (request.selectedProfile == null && request.requestUser)
            {
                var users = from u in db.Users where u.Id == userId select u;
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

            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]
        public ActionResult<string> Validate([FromBody] PostValidateRequest request)
        {
            IQueryable<Tokens> tokens = null;
            if (request.clientToken == null)
            {
                tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.Status == 2 select t;
            }
            else
            {
                tokens = from t in db.Tokens where t.AccessToken == request.accessToken && t.ClientToken == request.clientToken && t.Status == 2 select t;
            }

            if (tokens.Count() != 1)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]{HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} vaild token failed.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidToken());
            }
            else
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]{HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} vaild token successful.");
                return StatusCode((int)HttpStatusCode.NoContent);
            }
        }

        [HttpPost]
        public ActionResult<string> Invalidate([FromBody] PostInvalidateRequest request)
        {
            var tokens = from t in db.Tokens where t.AccessToken == request.accessToken select t;
            if (tokens.Count() == 1)
            {
                tokens.First().Status = 0;
                db.SaveChanges();
            }
            log.Info($"[ID: {HttpContext.Connection.Id}]{HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} with token {request.accessToken} was invalidated.");
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPost]
        public ActionResult<string> Signout([FromBody] PostSignoutRequest request)
        {
            log.Info($"[ID: {HttpContext.Connection.Id}]Got logout request from {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} with user {request.username}.");

            // Check if user exists.
            var users = from u in db.Users where u.Email == request.username select u;
            if (users.Count() != 1)
            {
                log.Info($"[ID: {HttpContext.Connection.Id}]Request user is not exists.");
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidUsername());
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
                return StatusCode((int)HttpStatusCode.Forbidden, ExcepitonWorker.InvalidPassword());
            }

            // Update cooldown.
            cooldown.LastLoginTime = time;
            cooldown.TryTimes = 0;
            db.SaveChanges();
            log.Info($"[ID: {HttpContext.Connection.Id}]Cooldown of user {user.Username} has reseted.");

            return StatusCode((int)HttpStatusCode.NoContent);
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

        public struct PostValidateRequest
        {
            public string accessToken;
            public string clientToken;
        }

        public struct PostInvalidateRequest
        {
            public string accessToken;
            public string clientToken;
        }

        public struct PostSignoutRequest
        {
            public string username;
            public string password;
        }
    }
}