using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class RewardsController : Controller
    {
        private readonly BFASDbContext _context;

        public RewardsController(BFASDbContext context)
        {
            _context = context;
        }

        // GET: Rewards/Index
        public async Task<IActionResult> Index()
        {
            return await MyRewards();
        }

        // GET: Rewards/MyRewards
        public async Task<IActionResult> MyRewards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            
            // Get or create reward record
            var reward = await _context.Rewards
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reward == null)
            {
                reward = new Reward
                {
                    UserId = userId.Value,
                    Points = 0,
                    Tier = "Bronze",
                    CashbackEarned = 0,
                    LastUpdated = DateTime.Now
                };
                _context.Rewards.Add(reward);
                await _context.SaveChangesAsync();
            }

            // Get transaction history for rewards
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account != null)
            {
                var recentTransactions = await _context.Transactions
                    .Where(t => t.AccountId == account.AccountId)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(10)
                    .ToListAsync();

                ViewBag.RecentTransactions = recentTransactions;
            }

            ViewBag.Balance = account?.Balance;
            
            return View("Index", reward);
        }

        // POST: Rewards/RedeemPoints
        [HttpPost]
        public async Task<IActionResult> RedeemPoints(int points)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var reward = await _context.Rewards
                .FirstOrDefaultAsync(r => r.UserId == userId);

            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (reward == null || account == null)
            {
                TempData["Error"] = "Reward account not found";
                return RedirectToAction("Index");
            }

            if (reward.Points < points)
            {
                TempData["Error"] = "Insufficient points";
                return RedirectToAction("Index");
            }

            // Convert points to cash (100 points = $1)
            decimal cashValue = points / 100m;

            // Deduct points
            reward.Points -= points;
            
            // Add cash to account
            account.Balance += cashValue;
            
            // Update cashback earned
            reward.CashbackEarned += cashValue;
            reward.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Redeemed {points} points for ${cashValue:N2}!";
            return RedirectToAction("Index");
        }

        // Helper method to add points (called from transaction controllers)
        public static async Task AddPointsForTransaction(BFASDbContext context, int userId, decimal amount)
        {
            var reward = await context.Rewards
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (reward == null)
            {
                reward = new Reward
                {
                    UserId = userId,
                    Points = 0,
                    Tier = "Bronze",
                    CashbackEarned = 0,
                    LastUpdated = DateTime.Now
                };
                context.Rewards.Add(reward);
            }

            // Award points: $1 = 1 point
            int pointsEarned = (int)amount;
            reward.Points += pointsEarned;

            // Calculate cashback based on tier
            decimal cashback = amount * reward.CashbackRate;
            reward.CashbackEarned += cashback;

            // Update tier based on points
            UpdateTier(reward);

            reward.LastUpdated = DateTime.Now;
            await context.SaveChangesAsync();
        }

        private static void UpdateTier(Reward reward)
        {
            if (reward.Points >= 10000)
                reward.Tier = "Platinum";
            else if (reward.Points >= 5000)
                reward.Tier = "Gold";
            else if (reward.Points >= 1000)
                reward.Tier = "Silver";
            else
                reward.Tier = "Bronze";
        }
    }
}
