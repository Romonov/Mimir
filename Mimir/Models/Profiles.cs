using System;
using System.Collections.Generic;

namespace Mimir.Models
{
    public partial class Profiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uuid { get; set; }
        public int Uid { get; set; }
        public byte SkinModel { get; set; }
        public string SkinUrl { get; set; }
        public string CapeUrl { get; set; }
        public byte IsSelected { get; set; }
    }
}
