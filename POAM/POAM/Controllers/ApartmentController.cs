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
    public class ApartmentController : Controller
    {
        private readonly POAMDbContext _context;

        public ApartmentController(POAMDbContext context)
        {
            _context = context;
        }

        [Route("ApartmentsOwned")]
        public async Task<IActionResult> ApartmentsOwned()
        {
            // a user can see owned apartments only if logged
            if (Authentication.Instance.isLoggedIn())
            {
                var apartments = await _context.Apartment.Where(a => a.IdOwner == Authentication.Instance.getCurrentUser().IdOwner).ToListAsync();
                return View(apartments);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        [Route("MainPage")]
        public async Task<IActionResult> MainPage()
        {
            // only logged users can see the main page
            if (Authentication.Instance.isLoggedIn())
            {

                var apartments = _context.Apartment.Include(a => a.IdOwnerNavigation).OrderBy(a=> a.IdOwnerNavigation.FullName);

                var contracts = _context.Contract;

                var employees = _context.Employee;

                // total salary of all employees
                double salarySum = 0;

                foreach(var employee in employees)
                {
                    salarySum += employee.Salary;
                }

                //number of all association tenants
                byte totalAssociationTenants = 0;

                foreach(var apartment in apartments)
                {
                    if (apartment.NoTenants != null)
                    {
                        totalAssociationTenants += apartment.NoTenants.Value;
                    }
                }

                // compute personnel salary tax per tenant
                double taxPerTenant = salarySum / totalAssociationTenants;

                foreach (Apartment apartment in apartments)
                {
                    var waterConsumptions = _context.WaterConsumption.Where(w => w.IdApartment == apartment.IdApartment);

                    var receipts = _context.Receipt.Where(r => r.IdApartment == apartment.IdApartment);

                    DateTime date = DateTime.Now;

                    var oldWaterConsumption = waterConsumptions.FirstOrDefault(w => w.Date.Month != date.Month);

                    // if an old report of water consumption was found
                    if(oldWaterConsumption != null)
                    {
                        // current debt becomes previous debt
                        apartment.PreviousDebt += apartment.CurrentDebt;
                        
                    }

                    apartment.CurrentDebt = 0;

                    var currentWaterConsumption = waterConsumptions.FirstOrDefault(w => w.Date.Month == date.Month);

                    // compute payment for warm and cold water in the current month
                    if(currentWaterConsumption != null)
                    {
                        var warmWaterContract = contracts.FirstOrDefault(c => c.Type == "Warm Water");
                        var coldWaterContract = contracts.FirstOrDefault(c => c.Type == "Cold Water");
                        apartment.CurrentDebt += currentWaterConsumption.ColdWater*coldWaterContract.Price + currentWaterConsumption.WarmWater * warmWaterContract.Price;

                    }

                    // for other contracts add tax per tenant
                    foreach(var contract in contracts)
                    {
                        if(contract.Type != "Water")
                        {
                            apartment.CurrentDebt += contract.Price * apartment.NoTenants;
                        }
                    }

                    // add personnel salary tax per tenant
                    apartment.CurrentDebt += apartment.NoTenants * taxPerTenant;

                    //subtract amount from current debt based on receipts
                    var currentReceipts = receipts.Where(r => r.Date.Month == date.Month);

                    foreach(var receipt in currentReceipts)
                    {
                        if(apartment.CurrentDebt != 0)
                        {
                            apartment.CurrentDebt -= receipt.Amount;
                        }
                        else
                        {
                            if(apartment.PreviousDebt != 0)
                            {
                                apartment.PreviousDebt -= receipt.Amount;
                            }
                        }
                    }
                   
                    apartment.CurrentDebt = Math.Round((double)apartment.CurrentDebt, 2);
                    apartment.PreviousDebt = Math.Round((double)apartment.PreviousDebt, 2);


                    apartment.TotalDebt = apartment.PreviousDebt + apartment.CurrentDebt;


                    _context.Update(apartment);
                    

                }

                await _context.SaveChangesAsync();
                return View(await apartments.ToListAsync());
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        
        public async Task<IActionResult> ApartmentDetails(int? id)
        {
            // only logged users can see apartment details
            if (Authentication.Instance.isLoggedIn())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var apartment = await _context.Apartment
                    .Include(a => a.IdOwnerNavigation)
                    .FirstOrDefaultAsync(m => m.IdApartment == id);
                if (apartment == null)
                {
                    return NotFound();
                }

                return View(apartment);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }


        public IActionResult AddApartment()
        {
            // only logged users can add owned apartments
            if (Authentication.Instance.isLoggedIn())
            {
                
                return View();
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddApartment([Bind("IdApartment,Street,Building,FlatNo,NoTenants,PreviousDebt,CurrentDebt,TotalDebt,IdOwner")] Apartment apartment)
        {
            // only logged users can add owned apartments
            if (Authentication.Instance.isLoggedIn())
            {

                if (ModelState.IsValid)
                {
                    Owner owner = Authentication.Instance.getCurrentUser();
                    apartment.IdOwner = owner.IdOwner;
                    apartment.CurrentDebt = 0;
                    apartment.PreviousDebt = 0;
                    apartment.TotalDebt = 0;
                    _context.Add(apartment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MainPage));
                }

                return View(apartment);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public async Task<IActionResult> DeleteApartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .Include(a => a.IdOwnerNavigation)
                .FirstOrDefaultAsync(m => m.IdApartment == id);
            if (apartment == null)
            {
                return NotFound();
            }
            // only the owner of the apartment can delete their apartment
            if (Authentication.Instance.isLoggedIn() && apartment.IdOwner == Authentication.Instance.getCurrentUser().IdOwner)
            {
                return View(apartment);
            }
            else
            {
                return Redirect("~/ApartmentsOwned");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApartment(int id)
        {

            var apartment = await _context.Apartment.FindAsync(id);
            // only the owner of the apartment can delete their apartment
            if (Authentication.Instance.isLoggedIn() && apartment.IdOwner == Authentication.Instance.getCurrentUser().IdOwner)
            {
              
                _context.Apartment.Remove(apartment);
                await _context.SaveChangesAsync();
                return Redirect("~/ApartmentsOwned");
            }
            else
            {
                return Redirect("~/ApartmentsOwned");
            }
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartment.Any(e => e.IdApartment == id);
        }

    }
}
