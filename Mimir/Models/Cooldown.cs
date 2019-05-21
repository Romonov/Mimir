using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Cooldown
    {
        public int Uid { get; set; }
        public int TryTimes { get; set; }
        public string LastTryTime { get; set; }
        public string LastLoginTime { get; set; }
        public int CooldownLevel { get; set; }
        public string CooldownTime { get; set; }
    }
}
