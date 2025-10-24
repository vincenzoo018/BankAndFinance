using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Helpers;
using BankAndFinance.Models;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class CustomerController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IBankingService _bankingService;
        private readonly IAuditLogService _auditLog;

        public CustomerController(BFASDbContext context, IBankingService bankingService, IAuditLogService auditLog)
        {
            _context = context;
            _bankingService = bankingService;
            _auditLog = auditLog;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Message = "No bank account found. Please contact admin.";
                return View();
            }

            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.AccountType = account.AccountType;
            ViewBag.Balance = account.Balance;

            var recentTransactions = await _context.Transactions
                .Where(t => t.AccountId == account.AccountId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentTransactions = recentTransactions;

            return View();
        }

        public async Task<IActionResult> Transactions()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.AccountNumber = account.AccountNumber;
            ViewBag.Balance = account.Balance;

            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.AccountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Deposit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.AccountNumber = account?.AccountNumber;
            ViewBag.CurrentBalance = account?.Balance;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(decimal amount, string paymentMode, string? qrCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                return View();
            }

            var result = await _bankingService.DepositAsync(account.AccountId, amount);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Deposit");
            }

            // Update transaction with payment mode and QR code
            var transaction = await _context.Transactions
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
            if (transaction != null)
            {
                transaction.PaymentMode = paymentMode;
                transaction.QRCode = qrCode;
                await _context.SaveChangesAsync();
            }

            await _auditLog.LogAsync($"Deposited ${amount:N2} via {paymentMode}", "Banking");
            TempData["Success"] = $"Successfully deposited ${amount:N2} via {paymentMode}!";

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Withdraw()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.AccountNumber = account?.AccountNumber;
            ViewBag.CurrentBalance = account?.Balance;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(decimal amount, string paymentMode, string? qrCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                return View();
            }

            var result = await _bankingService.WithdrawAsync(account.AccountId, amount);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Withdraw");
            }

            // Update transaction with payment mode and QR code
            var transaction = await _context.Transactions
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
            if (transaction != null)
            {
                transaction.PaymentMode = paymentMode;
                transaction.QRCode = qrCode;
                await _context.SaveChangesAsync();
            }

            await _auditLog.LogAsync($"Withdrew ${amount:N2} via {paymentMode}", "Banking");
            TempData["Success"] = $"Successfully withdrew ${amount:N2} via {paymentMode}!";

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.AccountNumber = account?.AccountNumber;
            ViewBag.CurrentBalance = account?.Balance;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(string receiverAccountNumber, decimal amount, string paymentMode, string? qrCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var senderAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (senderAccount == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                return View();
            }

            var result = await _bankingService.TransferAsync(senderAccount.AccountId, receiverAccountNumber, amount);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Transfer");
            }

            // Update transaction with payment mode and QR code
            var transaction = await _context.Transactions
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync(t => t.AccountId == senderAccount.AccountId);
            if (transaction != null)
            {
                transaction.PaymentMode = paymentMode;
                transaction.QRCode = qrCode;
                await _context.SaveChangesAsync();
            }

            await _auditLog.LogAsync($"Transferred ${amount:N2} to {receiverAccountNumber} via {paymentMode}", "Banking");
            TempData["Success"] = $"Successfully transferred ${amount:N2} via {paymentMode}!";

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> PayBills()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.AccountNumber = account?.AccountNumber;
            ViewBag.CurrentBalance = account?.Balance;
            ViewBag.Billers = await _context.Billers.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayBills(int billerId, decimal amount, string paymentMode, string? qrCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                ViewBag.Billers = await _context.Billers.ToListAsync();
                return View();
            }

            var result = await _bankingService.PayBillAsync(account.AccountId, billerId, amount);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("PayBills");
            }

            // Update transaction with payment mode and QR code
            var transaction = await _context.Transactions
                .OrderByDescending(t => t.TransactionId)
                .FirstOrDefaultAsync(t => t.AccountId == account.AccountId);
            if (transaction != null)
            {
                transaction.PaymentMode = paymentMode;
                transaction.QRCode = qrCode;
                await _context.SaveChangesAsync();
            }

            var biller = await _context.Billers.FindAsync(billerId);
            await _auditLog.LogAsync($"Paid bill of ${amount:N2} to {biller?.BillerName} via {paymentMode}", "Banking");
            TempData["Success"] = $"Successfully paid ${amount:N2} to {biller?.BillerName} via {paymentMode}!";

            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            // Pass balance to layout
            var account = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.UserId == userId);
            ViewBag.Balance = account?.Balance ?? 0;

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string email, string phone, DateTime? dateOfBirth, string address)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            
            // Update user information
            var user = await _context.Users
                .Include(u => u.CustomerProfile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Profile");
            }

            user.FullName = fullName;
            user.Email = email;

            // Update or create customer profile
            if (user.CustomerProfile == null)
            {
                user.CustomerProfile = new CustomerProfile
                {
                    UserId = userId!.Value,
                    Phone = phone,
                    Address = address,
                    DateOfBirth = dateOfBirth
                };
                _context.CustomerProfiles.Add(user.CustomerProfile);
            }
            else
            {
                user.CustomerProfile.Phone = phone;
                user.CustomerProfile.Address = address;
                user.CustomerProfile.DateOfBirth = dateOfBirth;
            }

            await _context.SaveChangesAsync();

            // Update session
            HttpContext.Session.SetString("UserName", fullName);

            // Log the action
            await _auditLog.LogAsync("Updated profile information", "Profile");

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}
