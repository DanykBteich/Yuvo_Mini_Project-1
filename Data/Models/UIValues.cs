using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Data.Models
{
    public class UIValues
    {
        public DateTime Date { get; set; }
        public string Link { get; set; }
        public string Slot { get; set; }
        public string NeAlias { get; set; }
        public string NeType { get; set; }
        public float MaxRXLevel { get; set; }
        public float MaxTXLevel { get; set; }
        public float RSLDeviation { get; set; }
    }
}
