using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Tokens
    {
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }
        public int? BindProfileId { get; set; }
        public string CreateTime { get; set; }
        public byte Status { get; set; }
    }
}
