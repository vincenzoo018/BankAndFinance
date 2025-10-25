using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Helpers;
using BankAndFinance.Models;
using BankAndFinance.Services;

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

        public async Task<IActionResult> Cards()
        {
            var cards = await _context.Cards
                .Include(c => c.Account)
                    .ThenInclude(a => a!.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(cards);
        }

        public async Task<IActionResult> CardDetails(int id)
        {
            var card = await _context.Cards
                .Include(c => c.Account)
                    .ThenInclude(a => a!.User)
                .FirstOrDefaultAsync(c => c.CardId == id);

            if (card == null)
            {
                TempData["Error"] = "Card not found";
                return RedirectToAction("Cards");
            }

            // Get transactions for this card
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == card.AccountId && 
                           (t.PaymentMode != null && t.PaymentMode.Contains(card.CardNumber.Substring(card.CardNumber.Length - 4))))
                .OrderByDescending(t => t.TransactionDate)
                .Take(50)
                .ToListAsync();

            ViewBag.Transactions = transactions;
            return View(card);
        }

        public async Task<IActionResult> CardRequests()
        {
            var requests = await _context.CardRequests
                .Include(cr => cr.User)
                .Include(cr => cr.Account)
                .Include(cr => cr.Approver)
                .OrderByDescending(cr => cr.CreatedAt)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveCardRequest(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cardService = HttpContext.RequestServices.GetRequiredService<ICardService>();
            var result = await cardService.ApproveCardRequestAsync(requestId, userId.Value);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("CardRequests");
        }

        [HttpPost]
        public async Task<IActionResult> RejectCardRequest(int requestId, string rejectionReason)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cardService = HttpContext.RequestServices.GetRequiredService<ICardService>();
            var result = await cardService.RejectCardRequestAsync(requestId, userId.Value, rejectionReason);

            if (result.Success)
            {
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("CardRequests");
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

        // Employee Management Methods
        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match";
                return View();
            }

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                TempData["Error"] = "Email address is already registered";
                return View();
            }

            // Hash password
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                password = Convert.ToBase64String(hashedBytes);
            }

            // Create new employee
            var employee = new User
            {
                RoleId = 2, // Employee role
                FullName = fullName,
                Email = email,
                Password = password,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Created new employee: {fullName}",
                Module = "Employee Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Employee created successfully";
            return RedirectToAction("Employees");
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var employee = await _context.Users.FindAsync(id);
            if (employee == null || employee.RoleId != 2)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Employees");
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(int userId, string fullName, string email)
        {
            var employee = await _context.Users.FindAsync(userId);
            if (employee == null || employee.RoleId != 2)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Employees");
            }

            // Check if email is being changed and if it's already taken by another user
            if (employee.Email != email)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserId != userId);
                if (existingUser != null)
                {
                    TempData["Error"] = "Email address is already taken by another user";
                    return View(employee);
                }
            }

            employee.FullName = fullName;
            employee.Email = email;

            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Updated employee: {fullName}",
                Module = "Employee Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Employee updated successfully";
            return RedirectToAction("Employees");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleEmployeeStatus(int userId, string status)
        {
            var employee = await _context.Users.FindAsync(userId);
            if (employee == null || employee.RoleId != 2)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Employees");
            }

            employee.Status = status;
            await _context.SaveChangesAsync();

            // Log the action
            var auditLog = new AuditLog
            {
                UserId = HttpContext.Session.GetInt32("UserId")!.Value,
                Action = $"Changed employee status to {status}: {employee.FullName}",
                Module = "Employee Management",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Employee {status.ToLower()} successfully";
            return RedirectToAction("Employees");
        }

        // API endpoint for transaction chart data
        [HttpGet]
        public async Task<IActionResult> GetTransactionData(string filterType, DateTime? startDate = null, DateTime? endDate = null)
        {
            DateTime start, end;
            
            // Determine date range based on filter type
            switch (filterType)
            {
                case "week":
                    end = DateTime.Now;
                    start = end.AddDays(-7);
                    break;
                case "year":
                    end = DateTime.Now;
                    start = new DateTime(end.Year, 1, 1);
                    break;
                case "custom":
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        start = startDate.Value;
                        end = endDate.Value.AddDays(1).AddSeconds(-1);
                    }
                    else
                    {
                        end = DateTime.Now;
                        start = new DateTime(end.Year, end.Month, 1);
                    }
                    break;
                default: // month
                    end = DateTime.Now;
                    start = new DateTime(end.Year, end.Month, 1);
                    break;
            }

            // Get transactions in date range
            var transactions = await _context.Transactions
                .Where(t => t.TransactionDate >= start && t.TransactionDate <= end)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            // Group by date
            var groupedData = transactions
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Deposits = g.Where(t => t.TransactionType.Contains("Deposit", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount),
                    Withdrawals = g.Where(t => t.TransactionType.Contains("Withdraw", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount),
                    Transfers = g.Where(t => t.TransactionType.Contains("Transfer", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount)
                })
                .OrderBy(g => g.Date)
                .ToList();

            // Format data for chart
            var labels = groupedData.Select(d => d.Date.ToString("MMM dd")).ToList();
            var deposits = groupedData.Select(d => d.Deposits).ToList();
            var withdrawals = groupedData.Select(d => d.Withdrawals).ToList();
            var transfers = groupedData.Select(d => d.Transfers).ToList();

            return Json(new
            {
                labels,
                deposits,
                withdrawals,
                transfers
            });
        }
    }
}
