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
    public class WaterConsumptionController : Controller
    {
        private readonly POAMDbContext _context;

        public WaterConsumptionController(POAMDbContext context)
        {
            _context = context;
        }

        [Route("WaterConsumption/AddWaterConsumption/{IdApartment}")]
        public IActionResult AddWaterConsumption()
        {         
            return View();
        }

        [Route("WaterConsumption/AddWaterConsumption/{IdApartment}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWaterConsumption([Bind("IdWaterConsumption,WarmWater,ColdWater,Date,IdApartment")] WaterConsumption waterConsumption)
        {
            var apartment = _context.Apartment.FirstOrDefault(a => a.IdApartment == waterConsumption.IdApartment);

            // owner of the apartment can add water consumption
            if (Authentication.Instance.isLoggedIn() && apartment.IdOwner == Authentication.Instance.getCurrentUser().IdOwner)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(waterConsumption);
                    await _context.SaveChangesAsync();
                    return Redirect("~/ApartmentsOwned");
                }
                return View(waterConsumption);
            }
            else
            {
                return Redirect("~/ApartmentsOwned");
            }
        }
    }
}
