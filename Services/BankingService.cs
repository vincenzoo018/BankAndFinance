using BankAndFinance.Data;
using BankAndFinance.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAndFinance.Services
{
    public interface IBankingService
    {
        Task<(bool Success, string Message)> DepositAsync(int accountId, decimal amount);
        Task<(bool Success, string Message)> WithdrawAsync(int accountId, decimal amount);
        Task<(bool Success, string Message)> TransferAsync(int senderAccountId, string receiverAccountNumber, decimal amount);
        Task<(bool Success, string Message)> PayBillAsync(int accountId, int billerId, decimal amount);
        string GenerateReferenceNumber(string prefix);
    }

    public class BankingService : IBankingService
    {
        private readonly BFASDbContext _context;

        public BankingService(BFASDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> DepositAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
                return (false, "Amount must be greater than zero.");

            var account = await _context.BankAccounts.FindAsync(accountId);
            if (account == null)
                return (false, "Account not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update balance
                account.Balance += amount;

                // Create transaction record
                var txn = new Transaction
                {
                    AccountId = accountId,
                    TransactionType = "Deposit",
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = GenerateReferenceNumber("DEP")
                };
                _context.Transactions.Add(txn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Deposit of ${amount:N2} completed successfully. New balance: ${account.Balance:N2}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Deposit failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> WithdrawAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
                return (false, "Amount must be greater than zero.");

            var account = await _context.BankAccounts.FindAsync(accountId);
            if (account == null)
                return (false, "Account not found.");

            if (account.Balance < amount)
                return (false, "Insufficient funds.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update balance
                account.Balance -= amount;

                // Create transaction record
                var txn = new Transaction
                {
                    AccountId = accountId,
                    TransactionType = "Withdraw",
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = GenerateReferenceNumber("WTD")
                };
                _context.Transactions.Add(txn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Withdrawal of ${amount:N2} completed successfully. New balance: ${account.Balance:N2}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Withdrawal failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> TransferAsync(int senderAccountId, string receiverAccountNumber, decimal amount)
        {
            if (amount <= 0)
                return (false, "Amount must be greater than zero.");

            var senderAccount = await _context.BankAccounts.FindAsync(senderAccountId);
            if (senderAccount == null)
                return (false, "Sender account not found.");

            if (senderAccount.Balance < amount)
                return (false, "Insufficient funds.");

            var receiverAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(a => a.AccountNumber == receiverAccountNumber);
            if (receiverAccount == null)
                return (false, "Receiver account not found.");

            if (senderAccount.AccountId == receiverAccount.AccountId)
                return (false, "Cannot transfer to the same account.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update balances
                senderAccount.Balance -= amount;
                receiverAccount.Balance += amount;

                // Create transfer record
                var transfer = new Transfer
                {
                    SenderAccountId = senderAccountId,
                    ReceiverAccountId = receiverAccount.AccountId,
                    Amount = amount,
                    TransferDate = DateTime.Now,
                    Status = "Completed"
                };
                _context.Transfers.Add(transfer);

                // Create transaction records
                var refNumber = GenerateReferenceNumber("TRF");

                var senderTxn = new Transaction
                {
                    AccountId = senderAccountId,
                    TransactionType = "Transfer Out",
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = refNumber
                };
                _context.Transactions.Add(senderTxn);

                var receiverTxn = new Transaction
                {
                    AccountId = receiverAccount.AccountId,
                    TransactionType = "Transfer In",
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = refNumber
                };
                _context.Transactions.Add(receiverTxn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Transfer of ${amount:N2} completed successfully. New balance: ${senderAccount.Balance:N2}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Transfer failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> PayBillAsync(int accountId, int billerId, decimal amount)
        {
            if (amount <= 0)
                return (false, "Amount must be greater than zero.");

            var account = await _context.BankAccounts.FindAsync(accountId);
            if (account == null)
                return (false, "Account not found.");

            if (account.Balance < amount)
                return (false, "Insufficient funds.");

            var biller = await _context.Billers.FindAsync(billerId);
            if (biller == null)
                return (false, "Biller not found.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update balance
                account.Balance -= amount;

                // Create payment record
                var payment = new Payment
                {
                    AccountId = accountId,
                    BillerId = billerId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = GenerateReferenceNumber("PAY")
                };
                _context.Payments.Add(payment);

                // Create transaction record
                var txn = new Transaction
                {
                    AccountId = accountId,
                    TransactionType = "Bill Payment",
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    Status = "Completed",
                    ReferenceNumber = payment.ReferenceNumber
                };
                _context.Transactions.Add(txn);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Bill payment of ${amount:N2} to {biller.BillerName} completed successfully. New balance: ${account.Balance:N2}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Bill payment failed: {ex.Message}");
            }
        }

        public string GenerateReferenceNumber(string prefix)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}-{timestamp}-{random}";
        }
    }
}
