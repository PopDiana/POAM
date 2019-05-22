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
    public class ContractController : Controller
    {
        private readonly POAMDbContext _context;

        public ContractController(POAMDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ContractsList()
        {
            // only admins can see the contract list
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                return View(await _context.Contract.ToListAsync());
            }
            else
            {
                return Redirect("~/MainPage");
            }
        
        }

        public async Task<IActionResult> ContractDetails(int? id)
        {
            // only admin can see contract details
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contract = await _context.Contract
                    .FirstOrDefaultAsync(m => m.IdContract == id);
                if (contract == null)
                {
                    return NotFound();
                }

                return View(contract);
            }
            else
            {
                return Redirect("~/MainPage");
            }
        }


        public IActionResult AddContract()
        {
            // only admins can add contracts
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                return View();
            }
            else
            {
                return Redirect("~/MainPage");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddContract([Bind("IdContract,Date,Provider,Price,Type")] Contract contract)
        {
            // only admins can add contracts
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                if (ModelState.IsValid)
                {
                    _context.Add(contract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ContractsList));
                }
                return View(contract);
            }
            else
            {
                return Redirect("~/MainPage");
            }
        }


        public async Task<IActionResult> FinalizeContract(int? id)
        {
            // only admins can finalize contracts
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var contract = await _context.Contract
                    .FirstOrDefaultAsync(m => m.IdContract == id);
                if (contract == null)
                {
                    return NotFound();
                }

                return View(contract);
            }
            else
            {
                return Redirect("~/MainPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeContract(int id)
        {
            // only admins can finalize contracts
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                var contract = await _context.Contract.FindAsync(id);
                _context.Contract.Remove(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ContractsList));
            }
            else
            {

                return Redirect("~/MainPage");
            }
        }

        private bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.IdContract == id);
        }
    }
}
