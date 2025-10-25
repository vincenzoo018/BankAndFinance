using BankAndFinance.Data;
using BankAndFinance.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAndFinance.Services
{
    public interface ICardService
    {
        Task<ServiceResult> RequestCardAsync(int userId, int accountId, string cardType, decimal? requestedLimit, string? reason);
        Task<ServiceResult> ApproveCardRequestAsync(int requestId, int approvedBy);
        Task<ServiceResult> RejectCardRequestAsync(int requestId, int rejectedBy, string reason);
        Task<ServiceResult> TransferBetweenCardsAsync(int fromCardId, int toCardId, decimal amount, string? description);
        Task<ServiceResult> SetPrimaryCardAsync(int cardId, int userId);
        Task<ServiceResult> BlockCardAsync(int cardId, int userId);
        Task<ServiceResult> UnblockCardAsync(int cardId, int userId);
        Task<ServiceResult> DepositToCardAsync(int cardId, decimal amount);
        Task<ServiceResult> WithdrawFromCardAsync(int cardId, decimal amount);
        Task<Card?> GetCardByIdAsync(int cardId);
        Task<List<Card>> GetUserCardsAsync(int userId);
    }

    public class CardService : ICardService
    {
        private readonly BFASDbContext _context;

        public CardService(BFASDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult> RequestCardAsync(int userId, int accountId, string cardType, decimal? requestedLimit, string? reason)
        {
            try
            {
                var account = await _context.BankAccounts.FindAsync(accountId);
                if (account == null || account.UserId != userId)
                {
                    return new ServiceResult { Success = false, Message = "Invalid account" };
                }

                var request = new CardRequest
                {
                    UserId = userId,
                    AccountId = accountId,
                    CardType = cardType,
                    RequestedLimit = requestedLimit,
                    Reason = reason,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.CardRequests.Add(request);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Card request submitted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> ApproveCardRequestAsync(int requestId, int approvedBy)
        {
            try
            {
                var request = await _context.CardRequests.FindAsync(requestId);
                if (request == null)
                {
                    return new ServiceResult { Success = false, Message = "Request not found" };
                }

                if (request.Status != "Pending")
                {
                    return new ServiceResult { Success = false, Message = "Request already processed" };
                }

                // Generate card number
                var cardNumber = GenerateCardNumber();

                // Create new card
                var card = new Card
                {
                    AccountId = request.AccountId,
                    CardNumber = cardNumber,
                    CardType = request.CardType,
                    CardStatus = "Active",
                    CVV = GenerateCVV(),
                    ExpiryDate = DateTime.Now.AddYears(3),
                    CardLimit = request.RequestedLimit,
                    Balance = 0,
                    IsPrimary = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Check if this is the first card for this account
                var existingCards = await _context.Cards
                    .Where(c => c.AccountId == request.AccountId)
                    .CountAsync();

                if (existingCards == 0)
                {
                    card.IsPrimary = true;
                }

                _context.Cards.Add(card);

                // Update request status
                request.Status = "Approved";
                request.ApprovedBy = approvedBy;
                request.ApprovalDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Card approved and created successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> RejectCardRequestAsync(int requestId, int rejectedBy, string reason)
        {
            try
            {
                var request = await _context.CardRequests.FindAsync(requestId);
                if (request == null)
                {
                    return new ServiceResult { Success = false, Message = "Request not found" };
                }

                if (request.Status != "Pending")
                {
                    return new ServiceResult { Success = false, Message = "Request already processed" };
                }

                request.Status = "Rejected";
                request.ApprovedBy = rejectedBy;
                request.ApprovalDate = DateTime.Now;
                request.RejectionReason = reason;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Card request rejected" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> TransferBetweenCardsAsync(int fromCardId, int toCardId, decimal amount, string? description)
        {
            try
            {
                if (amount <= 0)
                {
                    return new ServiceResult { Success = false, Message = "Amount must be greater than zero" };
                }

                var fromCard = await _context.Cards.FindAsync(fromCardId);
                var toCard = await _context.Cards.FindAsync(toCardId);

                if (fromCard == null || toCard == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (fromCard.CardStatus != "Active" || toCard.CardStatus != "Active")
                {
                    return new ServiceResult { Success = false, Message = "Card is not active" };
                }

                if (fromCard.Balance < amount)
                {
                    return new ServiceResult { Success = false, Message = "Insufficient card balance" };
                }

                // Perform transfer
                fromCard.Balance -= amount;
                fromCard.UpdatedAt = DateTime.Now;

                toCard.Balance += amount;
                toCard.UpdatedAt = DateTime.Now;

                // Record transfer
                var transfer = new CardTransfer
                {
                    FromCardId = fromCardId,
                    ToCardId = toCardId,
                    Amount = amount,
                    Description = description,
                    Status = "Completed",
                    TransferDate = DateTime.Now
                };

                _context.CardTransfers.Add(transfer);
                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = $"Successfully transferred ${amount:N2}" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> SetPrimaryCardAsync(int cardId, int userId)
        {
            try
            {
                var card = await _context.Cards
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.CardId == cardId);

                if (card == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (card.Account?.UserId != userId)
                {
                    return new ServiceResult { Success = false, Message = "Unauthorized" };
                }

                // Remove primary from all other cards
                var otherCards = await _context.Cards
                    .Where(c => c.AccountId == card.AccountId && c.CardId != cardId)
                    .ToListAsync();

                foreach (var otherCard in otherCards)
                {
                    otherCard.IsPrimary = false;
                    otherCard.UpdatedAt = DateTime.Now;
                }

                // Set this card as primary
                card.IsPrimary = true;
                card.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Primary card set successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> BlockCardAsync(int cardId, int userId)
        {
            try
            {
                var card = await _context.Cards
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.CardId == cardId);

                if (card == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (card.Account?.UserId != userId)
                {
                    return new ServiceResult { Success = false, Message = "Unauthorized" };
                }

                card.CardStatus = "Blocked";
                card.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Card blocked successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> UnblockCardAsync(int cardId, int userId)
        {
            try
            {
                var card = await _context.Cards
                    .Include(c => c.Account)
                    .FirstOrDefaultAsync(c => c.CardId == cardId);

                if (card == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (card.Account?.UserId != userId)
                {
                    return new ServiceResult { Success = false, Message = "Unauthorized" };
                }

                card.CardStatus = "Active";
                card.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = "Card unblocked successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> DepositToCardAsync(int cardId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    return new ServiceResult { Success = false, Message = "Amount must be greater than zero" };
                }

                var card = await _context.Cards.FindAsync(cardId);
                if (card == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (card.CardStatus != "Active")
                {
                    return new ServiceResult { Success = false, Message = "Card is not active" };
                }

                card.Balance += amount;
                card.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = $"Successfully deposited ${amount:N2}" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResult> WithdrawFromCardAsync(int cardId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    return new ServiceResult { Success = false, Message = "Amount must be greater than zero" };
                }

                var card = await _context.Cards.FindAsync(cardId);
                if (card == null)
                {
                    return new ServiceResult { Success = false, Message = "Card not found" };
                }

                if (card.CardStatus != "Active")
                {
                    return new ServiceResult { Success = false, Message = "Card is not active" };
                }

                if (card.Balance < amount)
                {
                    return new ServiceResult { Success = false, Message = "Insufficient card balance" };
                }

                card.Balance -= amount;
                card.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ServiceResult { Success = true, Message = $"Successfully withdrew ${amount:N2}" };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<Card?> GetCardByIdAsync(int cardId)
        {
            return await _context.Cards
                .Include(c => c.Account)
                    .ThenInclude(a => a!.User)
                .FirstOrDefaultAsync(c => c.CardId == cardId);
        }

        public async Task<List<Card>> GetUserCardsAsync(int userId)
        {
            return await _context.Cards
                .Include(c => c.Account)
                .Where(c => c.Account!.UserId == userId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        private string GenerateCardNumber()
        {
            var random = new Random();
            var cardNumber = "4"; // Visa starts with 4
            for (int i = 0; i < 15; i++)
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

    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
