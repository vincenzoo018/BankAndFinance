using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Admin", "Employee")]
    public class InventoryController : Controller
    {
        private readonly BFASDbContext _context;

        public InventoryController(BFASDbContext context)
        {
            _context = context;
        }

        // GET: Inventory/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            var items = await _context.Set<InventoryItem>()
                .OrderBy(i => i.ItemName)
                .ToListAsync();

            ViewBag.TotalItems = items.Count;
            ViewBag.LowStockItems = items.Count(i => i.Quantity <= i.ReorderLevel);
            ViewBag.TotalValue = items.Sum(i => i.Quantity * i.UnitPrice);

            return View(items);
        }

        // GET: Inventory/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                item.ItemCode = GenerateItemCode();
                item.CreatedAt = DateTime.Now;
                item.UpdatedAt = DateTime.Now;
                _context.Add(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inventory item added successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(item);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Set<InventoryItem>().FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.Balance = await GetUserBalanceAsync();
            return View(item);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryItem item)
        {
            if (id != item.ItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    item.UpdatedAt = DateTime.Now;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inventory item updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(item);
        }

        // POST: Inventory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Set<InventoryItem>().FindAsync(id);
            if (item != null)
            {
                _context.Set<InventoryItem>().Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inventory item deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Set<InventoryItem>().Any(e => e.ItemId == id);
        }

        private string GenerateItemCode()
        {
            var random = new Random();
            return $"ITM{DateTime.Now.Year}{random.Next(10000, 99999)}";
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
