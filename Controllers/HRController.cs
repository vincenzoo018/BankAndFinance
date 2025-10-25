using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Admin", "Employee")]
    public class HRController : Controller
    {
        private readonly BFASDbContext _context;

        public HRController(BFASDbContext context)
        {
            _context = context;
        }

        // GET: HR/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            var employees = await _context.Set<Employee>()
                .Include(e => e.User)
                .OrderBy(e => e.Department)
                .ToListAsync();

            ViewBag.TotalEmployees = employees.Count;
            ViewBag.ActiveEmployees = employees.Count(e => e.EmploymentStatus == "Active");
            ViewBag.Departments = employees.Select(e => e.Department).Distinct().Count();

            return View(employees);
        }

        // GET: HR/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            ViewBag.Users = await _context.Users.Where(u => u.RoleId == 2).ToListAsync();
            return View();
        }

        // POST: HR/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.EmployeeNumber = GenerateEmployeeNumber();
                employee.CreatedAt = DateTime.Now;
                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Employee added successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            ViewBag.Users = await _context.Users.Where(u => u.RoleId == 2).ToListAsync();
            return View(employee);
        }

        // GET: HR/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Set<Employee>().FindAsync(id);
            if (employee == null) return NotFound();

            ViewBag.Balance = await GetUserBalanceAsync();
            ViewBag.Users = await _context.Users.Where(u => u.RoleId == 2).ToListAsync();
            return View(employee);
        }

        // POST: HR/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Employee updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(employee);
        }

        // POST: HR/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Set<Employee>().FindAsync(id);
            if (employee != null)
            {
                _context.Set<Employee>().Remove(employee);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Employee deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Set<Employee>().Any(e => e.EmployeeId == id);
        }

        private string GenerateEmployeeNumber()
        {
            var random = new Random();
            return $"EMP{DateTime.Now.Year}{random.Next(1000, 9999)}";
        }

        private async Task<decimal> GetUserBalanceAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);
            return account?.Balance ?? 0;
        }
    }
}
