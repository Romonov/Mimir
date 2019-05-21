using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PreferredLanguage { get; set; }
        public string CreateIp { get; set; }
        public string CreateTime { get; set; }
    }
}
