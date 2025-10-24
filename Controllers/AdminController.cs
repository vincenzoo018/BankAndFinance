using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Helpers;
using BankAndFinance.Models;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private readonly BFASDbContext _context;

        public AdminController(BFASDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalAccounts = await _context.BankAccounts.CountAsync();
            ViewBag.TotalBalance = await _context.BankAccounts.SumAsync(ba => ba.Balance);
            ViewBag.TodayTransactions = await _context.Transactions
                .Where(t => t.TransactionDate.Date == DateTime.Today)
                .CountAsync();

            var recentTransactions = await _context.Transactions
                .Include(t => t.BankAccount)
                .OrderByDescending(t => t.TransactionDate)
                .Take(5)
                .ToListAsync();

            var recentActivities = await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentTransactions = recentTransactions;
            ViewBag.RecentActivities = recentActivities;

            return View();
        }

        // User Management
        public async Task<IActionResult> Users()
        {
            // Show only Customers (RoleId = 3)
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == 3)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Employees()
        {
            // Show only Employees (RoleId = 2)
            var employees = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == 2)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View(employees);
        }

        public async Task<IActionResult> Roles()
        {
            var roles = await _context.Roles.Include(r => r.Users).ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> CustomerProfiles()
        {
            var profiles = await _context.CustomerProfiles
                .Include(cp => cp.User)
                .ToListAsync();
            return View(profiles);
        }

        // Banking Management
        public async Task<IActionResult> BankAccounts()
        {
            var accounts = await _context.BankAccounts
                .Include(ba => ba.User)
                .ToListAsync();
            return View(accounts);
        }

        public async Task<IActionResult> Transactions()
        {
            var transactions = await _context.Transactions
                .Include(t => t.BankAccount)
                    .ThenInclude(ba => ba!.User)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
            return View(transactions);
        }

        public async Task<IActionResult> Transfers()
        {
            var transfers = await _context.Transfers
                .Include(t => t.SenderAccount)
                .Include(t => t.ReceiverAccount)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();
            return View(transfers);
        }

        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.BankAccount)
                .Include(p => p.Biller)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            return View(payments);
        }

        public async Task<IActionResult> Billers()
        {
            var billers = await _context.Billers.ToListAsync();
            return View(billers);
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

        public async Task<IActionResult> AccountsReceivable()
        {
            var receivables = await _context.AccountsReceivables
                .Include(ar => ar.Creator)
                .OrderByDescending(ar => ar.DueDate)
                .ToListAsync();
            return View(receivables);
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

        // Administration
        public async Task<IActionResult> AuditLogs()
        {
            var logs = await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
            return View(logs);
        }

        public async Task<IActionResult> Branches()
        {
            var branches = await _context.Branches.ToListAsync();
            return View(branches);
        }

        public IActionResult Settings()
        {
            return View();
        }

        // User Management Actions
        [HttpPost]
        public async Task<IActionResult> UpdateUser(int userId, string fullName, string email, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Users");
            }

            user.FullName = fullName;
            user.Email = email;
            user.RoleId = roleId;

            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Updated user: {fullName}",
                Module = "User Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User updated successfully";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int userId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Users");
            }

            user.Status = status;
            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Changed user status to {status}: {user.FullName}",
                Module = "User Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User {status.ToLower()} successfully";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerProfile(int profileId, string phone, string address, DateTime? dateOfBirth)
        {
            var profile = await _context.CustomerProfiles.FindAsync(profileId);
            if (profile == null)
            {
                TempData["Error"] = "Profile not found";
                return RedirectToAction("CustomerProfiles");
            }

            profile.Phone = phone;
            profile.Address = address;
            profile.DateOfBirth = dateOfBirth;

            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Updated customer profile ID: {profileId}",
                Module = "User Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Customer profile updated successfully";
            return RedirectToAction("CustomerProfiles");
        }
    }
}
