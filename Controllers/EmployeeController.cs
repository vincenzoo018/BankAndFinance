using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Helpers;
using BankAndFinance.Models;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Employee")]
    public class EmployeeController : Controller
    {
        private readonly BFASDbContext _context;

        public EmployeeController(BFASDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            ViewBag.PendingPayables = await _context.AccountsPayables
                .Where(ap => ap.Status == "Pending")
                .CountAsync();

            ViewBag.PendingReceivables = await _context.AccountsReceivables
                .Where(ar => ar.Status == "Pending")
                .CountAsync();

            ViewBag.TotalJournalEntries = await _context.JournalEntries
                .Where(je => je.CreatedBy == userId)
                .CountAsync();

            ViewBag.TodayTransactions = await _context.Transactions
                .Where(t => t.TransactionDate.Date == DateTime.Today)
                .CountAsync();

            var recentPayables = await _context.AccountsPayables
                .Where(ap => ap.Status == "Pending")
                .OrderBy(ap => ap.DueDate)
                .Take(5)
                .ToListAsync();

            var recentReceivables = await _context.AccountsReceivables
                .Where(ar => ar.Status == "Pending")
                .OrderBy(ar => ar.DueDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentPayables = recentPayables;
            ViewBag.RecentReceivables = recentReceivables;

            return View();
        }

        // Finance Management
        public async Task<IActionResult> AccountsPayable()
        {
            var payables = await _context.AccountsPayables
                .Include(ap => ap.Creator)
                .OrderByDescending(ap => ap.DueDate)
                .ToListAsync();
            return View(payables);
        }

        [HttpGet]
        public IActionResult CreateAccountPayable()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountPayable(AccountsPayable model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                model.CreatedBy = userId!.Value;
                
                _context.AccountsPayables.Add(model);
                await _context.SaveChangesAsync();

                // Log the action
                var auditLog = new AuditLog
                {
                    UserId = userId.Value,
                    Action = "Created Account Payable",
                    Module = "Finance",
                    Timestamp = DateTime.Now
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return RedirectToAction("AccountsPayable");
            }
            return View(model);
        }

        public async Task<IActionResult> AccountsReceivable()
        {
            var receivables = await _context.AccountsReceivables
                .Include(ar => ar.Creator)
                .OrderByDescending(ar => ar.DueDate)
                .ToListAsync();
            return View(receivables);
        }

        [HttpGet]
        public IActionResult CreateAccountReceivable()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountReceivable(AccountsReceivable model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                model.CreatedBy = userId!.Value;
                
                _context.AccountsReceivables.Add(model);
                await _context.SaveChangesAsync();

                // Log the action
                var auditLog = new AuditLog
                {
                    UserId = userId.Value,
                    Action = "Created Account Receivable",
                    Module = "Finance",
                    Timestamp = DateTime.Now
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return RedirectToAction("AccountsReceivable");
            }
            return View(model);
        }

        // Accounting Management
        public async Task<IActionResult> JournalEntries()
        {
            var entries = await _context.JournalEntries
                .Include(je => je.Creator)
                .Include(je => je.JournalEntryLines)
                .OrderByDescending(je => je.Date)
                .ToListAsync();
            return View(entries);
        }

        [HttpGet]
        public IActionResult CreateJournalEntry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJournalEntry(JournalEntry model, List<JournalEntryLine> lines)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                model.CreatedBy = userId!.Value;
                
                _context.JournalEntries.Add(model);
                await _context.SaveChangesAsync();

                foreach (var line in lines)
                {
                    line.JournalId = model.JournalId;
                    _context.JournalEntryLines.Add(line);
                }
                await _context.SaveChangesAsync();

                // Log the action
                var auditLog = new AuditLog
                {
                    UserId = userId.Value,
                    Action = "Created Journal Entry",
                    Module = "Accounting",
                    Timestamp = DateTime.Now
                };
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return RedirectToAction("JournalEntries");
            }
            return View(model);
        }

        public async Task<IActionResult> LedgerAccounts()
        {
            var ledgers = await _context.LedgerAccounts
                .OrderBy(la => la.AccountName)
                .ToListAsync();
            return View(ledgers);
        }

        public async Task<IActionResult> TrialBalance()
        {
            var trialBalances = await _context.TrialBalances
                .OrderByDescending(tb => tb.Period)
                .ToListAsync();
            return View(trialBalances);
        }

        public async Task<IActionResult> FinancialStatements()
        {
            var statements = await _context.FinancialStatements
                .OrderByDescending(fs => fs.CreatedAt)
                .ToListAsync();
            return View(statements);
        }
    }
}
