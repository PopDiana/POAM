using System;
using System.Collections.Generic;

namespace POAM.Models
{
    public partial class Receipt
    {
        public int IdReceipt { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public int IdApartment { get; set; }

        public Apartment IdApartmentNavigation { get; set; }
    }
}
