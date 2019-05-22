using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POAM.Models;

namespace POAM.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly POAMDbContext _context;

        public EmployeeController(POAMDbContext context)
        {
            _context = context;
        }


        [Route("Employees")]
        public async Task<IActionResult> EmployeesList()
        {
            // only admins can see the list of employees
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                return View(await _context.Employee.ToListAsync());
            }
            else
            {
                return Redirect("~/Home/Index");
            }
        }

        public IActionResult AddEmployee()
        {
            // only admins can add new employees
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
        public async Task<IActionResult> AddEmployee([Bind("IdEmployee,FullName,Address,Telephone,Employment,Salary,Pid")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EmployeesList));
            }
            return View(employee);
        }

        public async Task<IActionResult> EditEmployee(int? id)
        {
            // only admins can edit employees
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            else
            {
                return Redirect("~/MainPage");
            }
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, [Bind("IdEmployee,FullName,Address,Telephone,Employment,Salary,Pid")] Employee employee)
        {
            if (id != employee.IdEmployee)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.IdEmployee))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EmployeesList));
            }
            return View(employee);
        }

        public async Task<IActionResult> DeleteEmployee(int? id)
        {
            // only admins can delete employees
            if (Authentication.Instance.isLoggedIn() && Authentication.Instance.isAdmin())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee
                    .FirstOrDefaultAsync(m => m.IdEmployee == id);
                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
            else
            {
                return RedirectToAction(nameof(EmployeesList));
            }
        }

        [HttpPost, ActionName("DeleteEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployeeConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeesList));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.IdEmployee == id);
        }
    }
}
