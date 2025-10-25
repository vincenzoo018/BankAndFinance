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
        private readonly ICardService _cardService;

        public CardController(BFASDbContext context, IAuditLogService auditLog, ICardService cardService)
        {
            _context = context;
            _auditLog = auditLog;
            _cardService = cardService;
        }

        // GET: Card/MyCards
        public async Task<IActionResult> MyCards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cards = await _cardService.GetUserCardsAsync(userId.Value);
            
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            ViewBag.Balance = account?.Balance ?? 0;
            ViewBag.AccountNumber = account?.AccountNumber ?? "";
            ViewBag.TotalCardBalance = cards.Sum(c => c.Balance);
            
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
            
            // Get pending requests
            var pendingRequests = await _context.CardRequests
                .Where(cr => cr.UserId == userId && cr.Status == "Pending")
                .ToListAsync();
            ViewBag.PendingRequests = pendingRequests;
            
            return View();
        }

        // POST: Card/RequestCard
        [HttpPost]
        public async Task<IActionResult> RequestCard(string cardType, decimal? requestedLimit, string? reason)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Bank account not found";
                return RedirectToAction("MyCards");
            }

            var result = await _cardService.RequestCardAsync(userId.Value, account.AccountId, cardType, requestedLimit, reason);
            
            if (result.Success)
            {
                await _auditLog.LogAsync($"Requested new {cardType} card", "Card Management");
                TempData["Success"] = result.Message + " Waiting for admin approval.";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyCards");
        }

        // GET: Card/MyRequests
        public async Task<IActionResult> MyRequests()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var requests = await _context.CardRequests
                .Include(cr => cr.Account)
                .Include(cr => cr.Approver)
                .Where(cr => cr.UserId == userId)
                .OrderByDescending(cr => cr.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        // GET: Card/CardDetails
        public async Task<IActionResult> CardDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var card = await _context.Cards
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CardId == id && c.Account!.UserId == userId);

            if (card == null)
            {
                TempData["Error"] = "Card not found";
                return RedirectToAction("MyCards");
            }

            // Get transactions for this card
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == card.AccountId && 
                           t.PaymentMode != null && t.PaymentMode.Contains(card.CardNumber.Substring(card.CardNumber.Length - 4)))
                .OrderByDescending(t => t.TransactionDate)
                .Take(50)
                .ToListAsync();

            ViewBag.Transactions = transactions;
            return View(card);
        }

        // GET: Card/Transfer
        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var cards = await _cardService.GetUserCardsAsync(userId.Value);
            
            ViewBag.Cards = cards.Where(c => c.CardStatus == "Active").ToList();
            return View();
        }

        // POST: Card/Transfer
        [HttpPost]
        public async Task<IActionResult> Transfer(int fromCardId, int toCardId, decimal amount, string? description)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var result = await _cardService.TransferBetweenCardsAsync(fromCardId, toCardId, amount, description);

            if (result.Success)
            {
                await _auditLog.LogAsync($"Transferred ${amount:N2} between cards", "Card Transfer");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyCards");
        }

        // POST: Card/SetPrimary
        [HttpPost]
        public async Task<IActionResult> SetPrimary(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var result = await _cardService.SetPrimaryCardAsync(cardId, userId.Value);

            if (result.Success)
            {
                await _auditLog.LogAsync($"Set card as primary", "Card Management");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyCards");
        }

        // POST: Card/BlockCard
        [HttpPost]
        public async Task<IActionResult> BlockCard(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var result = await _cardService.BlockCardAsync(cardId, userId.Value);

            if (result.Success)
            {
                await _auditLog.LogAsync($"Blocked card", "Card Management");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction("MyCards");
        }

        // POST: Card/UnblockCard
        [HttpPost]
        public async Task<IActionResult> UnblockCard(int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var result = await _cardService.UnblockCardAsync(cardId, userId.Value);

            if (result.Success)
            {
                await _auditLog.LogAsync($"Unblocked card", "Card Management");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

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
