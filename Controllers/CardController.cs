using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class CardController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IAuditLogService _auditLog;

        public CardController(BFASDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        // GET: Card/MyCards
        public async Task<IActionResult> MyCards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Bank account not found";
                return RedirectToAction("Dashboard", "Customer");
            }

            var cards = await _context.Cards
                .Where(c => c.AccountId == account.AccountId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.Balance = account.Balance;
            ViewBag.AccountNumber = account.AccountNumber;
            
            return View(cards);
        }

        // GET: Card/RequestCard
        [HttpGet]
        public async Task<IActionResult> RequestCard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.AccountNumber = account?.AccountNumber;
            ViewBag.Balance = account?.Balance;
            
            return View();
        }

        // POST: Card/RequestCard
        [HttpPost]
        public async Task<IActionResult> RequestCard(string cardType)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Bank account not found";
                return RedirectToAction("MyCards");
            }

            // Check if user already has a card of this type
            var existingCard = await _context.Cards
                .FirstOrDefaultAsync(c => c.AccountId == account.AccountId && c.CardType == cardType && c.CardStatus == "Active");

            if (existingCard != null)
            {
                TempData["Error"] = $"You already have an active {cardType} card";
                return RedirectToAction("MyCards");
            }

            // Generate card number
            var cardNumber = GenerateCardNumber();
            var cvv = GenerateCVV();
            var expiryDate = DateTime.Now.AddYears(3);

            var card = new Card
            {
                AccountId = account.AccountId,
                CardNumber = cardNumber,
                CardType = cardType,
                CVV = cvv,
                ExpiryDate = expiryDate,
                CardStatus = "Active",
                CardLimit = cardType == "Debit" ? account.Balance : 5000,
                CreatedAt = DateTime.Now
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Requested new {cardType} card", "Card Management");

            TempData["Success"] = $"{cardType} card requested successfully! Your card will be delivered soon.";
            return RedirectToAction("MyCards");
        }

        // POST: Card/ActivateCard
        [HttpPost]
        public async Task<IActionResult> ActivateCard(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.CardId == cardId && c.AccountId == account.AccountId);

            if (card == null)
            {
                TempData["Error"] = "Card not found";
                return RedirectToAction("MyCards");
            }

            card.CardStatus = "Active";
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Activated card ending in {card.CardNumber.Substring(card.CardNumber.Length - 4)}", "Card Management");

            TempData["Success"] = "Card activated successfully!";
            return RedirectToAction("MyCards");
        }

        // POST: Card/DeactivateCard
        [HttpPost]
        public async Task<IActionResult> DeactivateCard(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.CardId == cardId && c.AccountId == account.AccountId);

            if (card == null)
            {
                TempData["Error"] = "Card not found";
                return RedirectToAction("MyCards");
            }

            card.CardStatus = "Inactive";
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Deactivated card ending in {card.CardNumber.Substring(card.CardNumber.Length - 4)}", "Card Management");

            TempData["Success"] = "Card deactivated successfully!";
            return RedirectToAction("MyCards");
        }

        // Helper methods
        private string GenerateCardNumber()
        {
            var random = new Random();
            var cardNumber = "4532"; // Visa prefix
            for (int i = 0; i < 12; i++)
            {
                cardNumber += random.Next(0, 10).ToString();
            }
            return cardNumber;
        }

        private string GenerateCVV()
        {
            var random = new Random();
            return random.Next(100, 999).ToString();
        }
    }
}
