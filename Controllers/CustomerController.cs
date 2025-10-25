using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class CustomerController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IBankingService _bankingService;
        private readonly IAuditLogService _auditLog;
        private readonly IWebHostEnvironment _environment;
        private readonly ICardService _cardService;

        public CustomerController(BFASDbContext context, IBankingService bankingService, IAuditLogService auditLog, IWebHostEnvironment environment, ICardService cardService)
        {
            _context = context;
            _bankingService = bankingService;
            _auditLog = auditLog;
            _environment = environment;
            _cardService = cardService;
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
            ViewBag.PaymentMode = "Cash"; // Only cash for deposits
            
            // Get user's active cards for deposit destination
            var cards = await _context.Cards
                .Where(c => c.AccountId == account.AccountId && c.CardStatus == "Active")
                .ToListAsync();
            ViewBag.Cards = cards;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(decimal amount, int? cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                ViewBag.Cards = new List<Card>();
                return View();
            }

            string paymentMode = "Cash Deposit";
            
            // If card is specified, deposit to card balance
            if (cardId.HasValue && cardId.Value > 0)
            {
                var cardResult = await _cardService.DepositToCardAsync(cardId.Value, amount);
                if (!cardResult.Success)
                {
                    TempData["Error"] = cardResult.Message;
                    return RedirectToAction("Deposit");
                }

                var card = await _context.Cards.FindAsync(cardId.Value);
                if (card != null)
                {
                    paymentMode = $"Cash Deposit to Card - {card.CardType} (**** {card.CardNumber.Substring(card.CardNumber.Length - 4)})";
                }
            }
            else
            {
                // Deposit to main account
                var result = await _bankingService.DepositAsync(account.AccountId, amount);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Deposit");
                }
            }

            // Create transaction record
            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = amount,
                TransactionType = "Deposit",
                TransactionDate = DateTime.Now,
                Status = "Completed",
                PaymentMode = paymentMode
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

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
            
            // Get user's active cards
            var cards = await _context.Cards
                .Where(c => c.AccountId == account.AccountId && c.CardStatus == "Active")
                .ToListAsync();
            
            ViewBag.Cards = cards;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(decimal amount, int? cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                ViewBag.Cards = new List<Card>();
                return View();
            }

            // Validate card selection
            if (!cardId.HasValue || cardId.Value == 0)
            {
                TempData["Error"] = "Please select a card for withdrawal";
                return RedirectToAction("Withdraw");
            }

            // Get the selected card
            var card = await _context.Cards.FindAsync(cardId.Value);
            if (card == null || card.AccountId != account.AccountId)
            {
                TempData["Error"] = "Invalid card selected";
                return RedirectToAction("Withdraw");
            }

            // Withdraw from card balance
            var cardResult = await _cardService.WithdrawFromCardAsync(cardId.Value, amount);
            if (!cardResult.Success)
            {
                TempData["Error"] = cardResult.Message;
                return RedirectToAction("Withdraw");
            }

            // Create transaction record
            var paymentMode = $"Card - {card.CardType} (**** {card.CardNumber.Substring(card.CardNumber.Length - 4)})";
            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = amount,
                TransactionType = "Withdrawal",
                TransactionDate = DateTime.Now,
                Status = "Completed",
                PaymentMode = paymentMode
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

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
            
            // Get user's active cards for payment
            var cards = await _context.Cards
                .Where(c => c.AccountId == account.AccountId && c.CardStatus == "Active")
                .ToListAsync();
            ViewBag.Cards = cards;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(string receiverAccountNumber, decimal amount, int? cardId, string? qrCode)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var senderAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (senderAccount == null)
            {
                ViewBag.Error = "Bank account not found";
                ViewBag.AccountNumber = "";
                ViewBag.CurrentBalance = 0;
                ViewBag.Cards = new List<Card>();
                return View();
            }

            // Validate card selection
            if (!cardId.HasValue || cardId.Value == 0)
            {
                TempData["Error"] = "Please select a card for this transfer";
                return RedirectToAction("Transfer");
            }

            // Get the selected card
            var card = await _context.Cards.FindAsync(cardId.Value);
            if (card == null || card.AccountId != senderAccount.AccountId)
            {
                TempData["Error"] = "Invalid card selected";
                return RedirectToAction("Transfer");
            }

            // Check card has sufficient balance
            if (card.Balance < amount)
            {
                TempData["Error"] = "Insufficient balance on selected card";
                return RedirectToAction("Transfer");
            }

            // Find receiver account
            var receiverAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.AccountNumber == receiverAccountNumber);

            if (receiverAccount == null)
            {
                TempData["Error"] = "Receiver account not found";
                return RedirectToAction("Transfer");
            }

            // Perform transfer from card balance
            var cardResult = await _cardService.WithdrawFromCardAsync(cardId.Value, amount);
            if (!cardResult.Success)
            {
                TempData["Error"] = cardResult.Message;
                return RedirectToAction("Transfer");
            }

            // Add to receiver's account
            receiverAccount.Balance += amount;

            // Create transfer record
            var transfer = new Transfer
            {
                SenderAccountId = senderAccount.AccountId,
                ReceiverAccountId = receiverAccount.AccountId,
                Amount = amount,
                TransferDate = DateTime.Now,
                Status = "Completed"
            };
            _context.Transfers.Add(transfer);

            // Create transaction records
            var paymentMode = $"Card - {card.CardType} (**** {card.CardNumber.Substring(card.CardNumber.Length - 4)})";
            
            var senderTransaction = new Transaction
            {
                AccountId = senderAccount.AccountId,
                Amount = amount,
                TransactionType = "Transfer Out",
                TransactionDate = DateTime.Now,
                Status = "Completed",
                PaymentMode = paymentMode,
                QRCode = qrCode
            };
            _context.Transactions.Add(senderTransaction);

            var receiverTransaction = new Transaction
            {
                AccountId = receiverAccount.AccountId,
                Amount = amount,
                TransactionType = "Transfer In",
                TransactionDate = DateTime.Now,
                Status = "Completed",
                PaymentMode = "Bank Transfer"
            };
            _context.Transactions.Add(receiverTransaction);

            await _context.SaveChangesAsync();

            // Create notifications for sender and receiver
            var receiverWithUser = await _context.BankAccounts
                .Include(ba => ba.User)
                .FirstOrDefaultAsync(ba => ba.AccountNumber == receiverAccountNumber);

            if (receiverWithUser != null)
            {
                // Notification for receiver
                var receiverNotification = new Notification
                {
                    UserId = receiverWithUser.UserId,
                    Title = "💰 Money Received",
                    Message = $"You received ${amount:N2} from {HttpContext.Session.GetString("UserName")} (Account: {senderAccount.AccountNumber})",
                    Type = "Success",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(receiverNotification);

                // Notification for sender
                var senderNotification = new Notification
                {
                    UserId = userId.Value,
                    Title = "✅ Transfer Successful",
                    Message = $"You transferred ${amount:N2} to {receiverWithUser.User?.FullName ?? receiverAccountNumber} (Account: {receiverAccountNumber})",
                    Type = "Info",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(senderNotification);

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
            
            // Get user's active cards
            var cards = await _context.Cards
                .Where(c => c.AccountId == account.AccountId && c.CardStatus == "Active")
                .ToListAsync();
            ViewBag.Cards = cards;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PayBills(int billerId, decimal amount, int? cardId)
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
                ViewBag.Cards = new List<Card>();
                return View();
            }

            // Validate card selection
            if (!cardId.HasValue || cardId.Value == 0)
            {
                TempData["Error"] = "Please select a card for payment";
                return RedirectToAction("PayBills");
            }

            // Get the selected card
            var card = await _context.Cards.FindAsync(cardId.Value);
            if (card == null || card.AccountId != account.AccountId)
            {
                TempData["Error"] = "Invalid card selected";
                return RedirectToAction("PayBills");
            }

            // Pay from card balance
            var cardResult = await _cardService.WithdrawFromCardAsync(cardId.Value, amount);
            if (!cardResult.Success)
            {
                TempData["Error"] = cardResult.Message;
                return RedirectToAction("PayBills");
            }

            // Create payment record
            var payment = new Payment
            {
                BillerId = billerId,
                AccountId = account.AccountId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                Status = "Completed"
            };
            _context.Payments.Add(payment);

            // Create transaction record
            var paymentMode = $"Card - {card.CardType} (**** {card.CardNumber.Substring(card.CardNumber.Length - 4)})";
            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = amount,
                TransactionType = "Bill Payment",
                TransactionDate = DateTime.Now,
                Status = "Completed",
                PaymentMode = paymentMode
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

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
        public async Task<IActionResult> UpdateProfile(string fullName, string email, string phone, DateTime? dateOfBirth, string address, IFormFile? profilePhoto)
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

            // Handle profile photo upload
            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profilePhoto.FileName).ToLower();
                
                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Only image files (JPG, PNG, GIF) are allowed.";
                    return RedirectToAction("Profile");
                }

                // Validate file size (max 5MB)
                if (profilePhoto.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "Profile photo must be less than 5MB.";
                    return RedirectToAction("Profile");
                }

                // Delete old photo if exists
                if (!string.IsNullOrEmpty(user.ProfilePhoto))
                {
                    var oldPhotoPath = Path.Combine(_environment.WebRootPath, user.ProfilePhoto.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePhoto.CopyToAsync(stream);
                }

                // Update user's profile photo path
                user.ProfilePhoto = $"/uploads/profiles/{fileName}";
            }

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
            HttpContext.Session.SetString("UserEmail", email);
            
            // Update profile photo in session
            if (!string.IsNullOrEmpty(user.ProfilePhoto))
            {
                HttpContext.Session.SetString("ProfilePhoto", user.ProfilePhoto);
            }

            // Log the action
            await _auditLog.LogAsync("Updated profile information" + (profilePhoto != null ? " with new photo" : ""), "Profile");

            TempData["Success"] = "Profile updated successfully!" + (profilePhoto != null ? " Your photo has been updated." : "");
            return RedirectToAction("Profile");
        }

        // GET: Customer/GetBalance (API for real-time balance updates)
        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            return Json(new { balance = account?.Balance ?? 0 });
        }
    }
}
