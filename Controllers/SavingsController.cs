using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Customer")]
    public class SavingsController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IAuditLogService _auditLog;

        public SavingsController(BFASDbContext context, IAuditLogService auditLog)
        {
            _context = context;
            _auditLog = auditLog;
        }

        // GET: Savings/Goals
        public async Task<IActionResult> Goals()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Bank account not found";
                return RedirectToAction("Dashboard", "Customer");
            }

            var goals = await _context.SavingsGoals
                .Where(g => g.AccountId == account.AccountId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            ViewBag.Balance = account.Balance;
            ViewBag.AccountId = account.AccountId;
            
            return View(goals);
        }

        // POST: Savings/CreateGoal
        [HttpPost]
        public async Task<IActionResult> CreateGoal(string goalName, decimal targetAmount, DateTime? targetDate)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            if (account == null)
            {
                TempData["Error"] = "Bank account not found";
                return RedirectToAction("Goals");
            }

            var goal = new SavingsGoal
            {
                AccountId = account.AccountId,
                GoalName = goalName,
                TargetAmount = targetAmount,
                CurrentAmount = 0,
                TargetDate = targetDate,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.SavingsGoals.Add(goal);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Created savings goal: {goalName}", "Savings");

            TempData["Success"] = "Savings goal created successfully!";
            return RedirectToAction("Goals");
        }

        // POST: Savings/AddToGoal
        [HttpPost]
        public async Task<IActionResult> AddToGoal(int goalId, decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.AccountId == account.AccountId);

            if (goal == null)
            {
                TempData["Error"] = "Goal not found";
                return RedirectToAction("Goals");
            }

            if (account.Balance < amount)
            {
                TempData["Error"] = "Insufficient balance";
                return RedirectToAction("Goals");
            }

            // Deduct from account
            account.Balance -= amount;

            // Add to goal
            goal.CurrentAmount += amount;

            // Check if goal reached
            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.Status = "Completed";
                TempData["Success"] = $"🎉 Congratulations! You've reached your '{goal.GoalName}' goal!";
            }
            else
            {
                TempData["Success"] = $"Added ${amount:N2} to your '{goal.GoalName}' goal!";
            }

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync($"Added ${amount:N2} to savings goal: {goal.GoalName}", "Savings");

            return RedirectToAction("Goals");
        }

        // POST: Savings/WithdrawFromGoal
        [HttpPost]
        public async Task<IActionResult> WithdrawFromGoal(int goalId, decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.AccountId == account.AccountId);

            if (goal == null || goal.CurrentAmount < amount)
            {
                TempData["Error"] = "Invalid withdrawal amount";
                return RedirectToAction("Goals");
            }

            // Add to account
            account.Balance += amount;

            // Deduct from goal
            goal.CurrentAmount -= amount;
            goal.Status = "Active"; // Reset status if withdrawn

            await _context.SaveChangesAsync();
            await _auditLog.LogAsync($"Withdrew ${amount:N2} from savings goal: {goal.GoalName}", "Savings");

            TempData["Success"] = $"Withdrew ${amount:N2} from your '{goal.GoalName}' goal!";
            return RedirectToAction("Goals");
        }

        // POST: Savings/DeleteGoal
        [HttpPost]
        public async Task<IActionResult> DeleteGoal(int goalId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);

            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.AccountId == account.AccountId);

            if (goal == null)
            {
                TempData["Error"] = "Goal not found";
                return RedirectToAction("Goals");
            }

            // Return money to account
            if (goal.CurrentAmount > 0)
            {
                account.Balance += goal.CurrentAmount;
            }

            _context.SavingsGoals.Remove(goal);
            await _context.SaveChangesAsync();

            await _auditLog.LogAsync($"Deleted savings goal: {goal.GoalName}", "Savings");

            TempData["Success"] = "Savings goal deleted successfully!";
            return RedirectToAction("Goals");
        }
    }
}
