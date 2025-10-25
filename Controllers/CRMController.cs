using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Admin", "Employee")]
    public class CRMController : Controller
    {
        private readonly BFASDbContext _context;

        public CRMController(BFASDbContext context)
        {
            _context = context;
        }

        // GET: CRM/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            var customers = await _context.Set<CRMCustomer>()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.TotalCustomers = customers.Count;
            ViewBag.QualifiedLeads = customers.Count(c => c.LeadStatus == "Qualified");
            ViewBag.TotalDealValue = customers.Where(c => c.DealValue.HasValue).Sum(c => c.DealValue.Value);

            return View(customers);
        }

        // GET: CRM/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            return View();
        }

        // POST: CRM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CRMCustomer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Customer added successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(customer);
        }

        // GET: CRM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Set<CRMCustomer>().FindAsync(id);
            if (customer == null) return NotFound();

            ViewBag.Balance = await GetUserBalanceAsync();
            return View(customer);
        }

        // POST: CRM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CRMCustomer customer)
        {
            if (id != customer.CRMCustomerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    customer.UpdatedAt = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Customer updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CRMCustomerId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(customer);
        }

        // POST: CRM/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Set<CRMCustomer>().FindAsync(id);
            if (customer != null)
            {
                _context.Set<CRMCustomer>().Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Customer deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Set<CRMCustomer>().Any(e => e.CRMCustomerId == id);
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
