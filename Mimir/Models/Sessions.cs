using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Sessions
    {
        public int Sid { get; set; }
        public string ServerId { get; set; }
        public string AccessToken { get; set; }
        public string ClientIp { get; set; }
    }
}
