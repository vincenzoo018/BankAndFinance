using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class BeneficiaryController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IAuditLogService _auditLog;

        public BeneficiaryController(BFASDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        // GET: Beneficiary/Index
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var beneficiaries = await _context.Beneficiaries
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.Balance = account?.Balance;
            return View(beneficiaries);
        }

        // POST: Beneficiary/Add
        [HttpPost]
        public async Task<IActionResult> Add(string beneficiaryName, string accountNumber, string? bankName, string? nickname)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Check if account exists
            var receiverAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.AccountNumber == accountNumber);

            if (receiverAccount == null)
            {
                TempData["Error"] = "Account number not found in our system";
                return RedirectToAction("Index");
            }

            // Check if already added
            var existing = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.UserId == userId && b.AccountNumber == accountNumber);

            if (existing != null)
            {
                TempData["Error"] = "This beneficiary is already in your list";
                return RedirectToAction("Index");
            }

            var beneficiary = new Beneficiary
            {
                UserId = userId.Value,
                BeneficiaryName = beneficiaryName,
                AccountNumber = accountNumber,
                BankName = bankName ?? "BFAS Bank",
                Nickname = nickname,
                CreatedAt = DateTime.Now
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Added beneficiary: {beneficiaryName}", "Beneficiary Management");

            TempData["Success"] = "Beneficiary added successfully!";
            return RedirectToAction("Index");
        }

        // POST: Beneficiary/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int beneficiaryId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.BeneficiaryId == beneficiaryId && b.UserId == userId);

            if (beneficiary == null)
            {
                TempData["Error"] = "Beneficiary not found";
                return RedirectToAction("Index");
            }

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Removed beneficiary: {beneficiary.BeneficiaryName}", "Beneficiary Management");

            TempData["Success"] = "Beneficiary removed successfully!";
            return RedirectToAction("Index");
        }

        // POST: Beneficiary/QuickTransfer
        [HttpPost]
        public async Task<IActionResult> QuickTransfer(int beneficiaryId, decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(b => b.BeneficiaryId == beneficiaryId && b.UserId == userId);

            if (beneficiary == null)
            {
                TempData["Error"] = "Beneficiary not found";
                return RedirectToAction("Index");
            }

            // Redirect to transfer page with pre-filled data
            TempData["PrefilledReceiver"] = beneficiary.AccountNumber;
            TempData["PrefilledAmount"] = amount;
            TempData["BeneficiaryName"] = beneficiary.Nickname ?? beneficiary.BeneficiaryName;

            return RedirectToAction("Transfer", "Customer");
        }
    }
}
