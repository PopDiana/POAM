using System;
using System.Collections.Generic;

namespace POAM.Models
{
    public partial class Apartment
    {
        public Apartment()
        {
            Receipt = new HashSet<Receipt>();
            WaterConsumption = new HashSet<WaterConsumption>();
        }

        public int IdApartment { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string FlatNo { get; set; }
        public byte? NoTenants { get; set; }
        public double? PreviousDebt { get; set; }
        public double? CurrentDebt { get; set; }
        public double? TotalDebt { get; set; }
        public int IdOwner { get; set; }

        public Owner IdOwnerNavigation { get; set; }
        public ICollection<Receipt> Receipt { get; set; }
        public ICollection<WaterConsumption> WaterConsumption { get; set; }
    }
}
