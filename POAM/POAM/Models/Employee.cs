using System;
using System.Collections.Generic;

namespace POAM.Models
{
    public partial class Employee
    {
        public int IdEmployee { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Employment { get; set; }
        public double Salary { get; set; }
        public string Pid { get; set; }
    }
}
