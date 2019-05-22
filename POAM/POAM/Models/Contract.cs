using System;
using System.Collections.Generic;

namespace POAM.Models
{
    public partial class Contract
    {
        public int IdContract { get; set; }
        public DateTime Date { get; set; }
        public string Provider { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
    }
}
