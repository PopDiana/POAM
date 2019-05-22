using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POAM.Models;

namespace POAM.Controllers
{
    public class ReceiptController : Controller
    {
        private readonly POAMDbContext _context;

        public ReceiptController(POAMDbContext context)
        {
            _context = context;
        }

        [Route("Receipt/AddPaymentReceipt/{IdApartment}")]
        public IActionResult AddPaymentReceipt()
        {
            return View();
        }

        [Route("Receipt/AddPaymentReceipt/{IdApartment}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPaymentReceipt([Bind("IdReceipt,Date,Amount,IdApartment")] Receipt receipt)
        {
            var apartment = _context.Apartment.FirstOrDefault(a => a.IdApartment == receipt.IdApartment);

            // owner of the apartment can add a payment receipt
            if (Authentication.Instance.isLoggedIn() && apartment.IdOwner == Authentication.Instance.getCurrentUser().IdOwner)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(receipt);
                    await _context.SaveChangesAsync();
                    return Redirect("~/ApartmentsOwned");
                }
                return View(receipt);
            }
            else
            {
                return Redirect("~/ApartmentsOwned");
            }
        }

    }
}
