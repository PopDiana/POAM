using System;
using System.Collections.Generic;

namespace POAM.Models
{
    public partial class WaterConsumption
    {
        public int IdWaterConsumption { get; set; }
        public int? WarmWater { get; set; }
        public int? ColdWater { get; set; }
        public DateTime Date { get; set; }
        public int IdApartment { get; set; }

        public Apartment IdApartmentNavigation { get; set; }
    }
}
