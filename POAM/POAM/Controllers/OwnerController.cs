using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POAM.Models;

namespace POAM.Controllers
{
    public class OwnerController : Controller
    {
        private readonly POAMDbContext _context;

        public OwnerController(POAMDbContext context)
        {
            _context = context;
        }

        [Route("Owners")]
        public async Task<IActionResult> OwnersList()
        {
            // get a list of all apartments to compute each owner's total debt
            ViewBag.apartments = _context.Apartment.ToList();

            // only logged users can see the list of owners
            if (Authentication.Instance.isLoggedIn())
            {
                return View(await _context.Owner.ToListAsync());
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }


        public IActionResult AddOwner()
        {
            // only admins can add new owners
            if (Authentication.Instance.isAdmin() && Authentication.Instance.isLoggedIn())
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
        public async Task<IActionResult> AddOwner([Bind("IdOwner,Username,Password,FullName,Telephone,Email,IsAdmin")] Owner owner)
        {
            // only admins can add new owners
            if (Authentication.Instance.isAdmin() && Authentication.Instance.isLoggedIn())
            {
                var existingOwner = _context.Owner.FirstOrDefault(o => o.Username == owner.Username);
                if (existingOwner != null)
                {
                    ModelState.AddModelError("", "The username already exists.");
                }
                if (ModelState.IsValid && existingOwner == null)
                {
                    // hash password

                    String salt = Authentication.Instance.GetRandomSalt();
                    owner.PassSalt = salt;
                    owner.Password = Authentication.Instance.HashPassword(owner.Password, salt);

                    _context.Add(owner);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(OwnersList));
                }
                return View(owner);
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }


        public async Task<IActionResult> EditOwner(int? id) {

            if (id == null)
            {
                return NotFound();
            }
            else if (Authentication.Instance.isLoggedIn())
            {
                // only owners can edit their data
                if (Authentication.Instance.getCurrentUser().IdOwner == id) { 
                    var owner = await _context.Owner.FindAsync(id);
                    if (owner == null)
                    {
                    
                    }
                     return View(owner);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Redirect("~/Owners");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOwner(int id, [Bind("IdOwner,Username,Password,FullName,Telephone,Email,IsAdmin")] Owner owner)
        {
            if (id != owner.IdOwner)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // rehash the new pasword

                    String salt = Authentication.Instance.GetRandomSalt();
                    owner.PassSalt = salt;
                    owner.Password = Authentication.Instance.HashPassword(owner.Password, salt);
                    owner.IsAdmin = Authentication.Instance.isAdmin();
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                    Authentication.Instance.Logout();
                    if(owner.IsAdmin == true)
                    {
                        Authentication.Instance.AdminLogin(owner);
                    }
                    else
                    {
                        Authentication.Instance.UserLogin(owner);
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.IdOwner))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(OwnersList));
            }
            return View(owner);
        }

        public async Task<IActionResult> DeleteOwner(int? id)
        {
            // only admins can delete an owner account
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {

                if (id == null)
                {
                    return NotFound();
                }

                var owner = await _context.Owner
                    .FirstOrDefaultAsync(m => m.IdOwner == id);

                if (owner == null)
                {
                    return NotFound();
                }

                return View(owner);
            }
            else
            {
                return RedirectToAction(nameof(OwnersList));
            }
        }

        [HttpPost, ActionName("DeleteOwner")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOwnerConfirmed(int id)
        {
            var owner = await _context.Owner.FindAsync(id);

           
            _context.Owner.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(OwnersList));
        }


        private bool OwnerExists(int id)
        {
            return _context.Owner.Any(e => e.IdOwner == id);
        }


        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(Owner user)
        {

            var dbUser = _context.Owner.FirstOrDefault(o => o.Username == user.Username);

            // search database for username and password
            if (dbUser == null)
            {
                ModelState.AddModelError("", "The username does not exist.");
                return View();
            }

            if (!Authentication.Instance.ValidatePassword(user.Password, dbUser.Password, dbUser.PassSalt))
            {
                ModelState.AddModelError("", "You introduced a wrong password.");
                return View();
            }

            // login as typical user or admin
            if (dbUser.IsAdmin == true)
            {
                Authentication.Instance.AdminLogin(dbUser);
            } else
            {
                Authentication.Instance.UserLogin(dbUser);
            }

            await _context.SaveChangesAsync();

            // redirect to main page after successful login
            return Redirect("~/MainPage");

        }


        public IActionResult Logout()
        {
            Authentication.Instance.Logout();
            return Redirect("~/Home/Index");
        }
    }
}
