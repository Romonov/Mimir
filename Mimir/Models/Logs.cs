using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Logs
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int? Pid { get; set; }
        public string Log { get; set; }
        public string Note { get; set; }
    }
}
