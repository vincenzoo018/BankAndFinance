using Microsoft.EntityFrameworkCore;
using BankAndFinance.Models;

namespace BankAndFinance.Data
{
    public class BFASDbContext : DbContext
    {
        public BFASDbContext(DbContextOptions<BFASDbContext> options) : base(options)
        {
        }

        // DbSets for all tables
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Biller> Billers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AccountsPayable> AccountsPayables { get; set; }
        public DbSet<AccountsReceivable> AccountsReceivables { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<LedgerAccount> LedgerAccounts { get; set; }
        public DbSet<TrialBalance> TrialBalances { get; set; }
        public DbSet<FinancialStatement> FinancialStatements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<AccountNumberGenerator> AccountNumberGenerators { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        
        // New Banking Features
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardRequest> CardRequests { get; set; }
        public DbSet<CardTransfer> CardTransfers { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<QRPayment> QRPayments { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<ScheduledPayment> ScheduledPayments { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        
        // ERP Modules
        public DbSet<Employee> Employees { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<CRMCustomer> CRMCustomers { get; set; }
        public DbSet<Project> Projects { get; set; }
        
        // Advanced Banking & Finance Features
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<FixedAsset> FixedAssets { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        
        // Customer ERP Features
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<CreditScore> CreditScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            
            // User - Role (Many-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - BankAccount (One-to-Many)
            modelBuilder.Entity<BankAccount>()
                .HasOne(ba => ba.User)
                .WithMany()
                .HasForeignKey(ba => ba.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - CustomerProfile (One-to-One)
            modelBuilder.Entity<CustomerProfile>()
                .HasOne(cp => cp.User)
                .WithOne(u => u.CustomerProfile)
                .HasForeignKey<CustomerProfile>(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BankAccount - Transactions (One-to-Many)
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.BankAccount)
                .WithMany(ba => ba.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer relationships
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.SenderAccount)
                .WithMany(ba => ba.SentTransfers)
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ReceiverAccount)
                .WithMany(ba => ba.ReceivedTransfers)
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.BankAccount)
                .WithMany(ba => ba.Payments)
                .HasForeignKey(p => p.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Biller)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BillerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Journal Entry Lines relationship
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(jel => jel.JournalEntry)
                .WithMany(je => je.JournalEntryLines)
                .HasForeignKey(jel => jel.JournalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Employee" },
                new Role { RoleId = 3, RoleName = "Customer" }
            );

            // Seed Users (Password should be hashed in production)
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    RoleId = 1, 
                    FullName = "Admin User", 
                    Email = "admin@bfassystem.com", 
                    Password = "Admin123", // Should be hashed
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 10, 20)
                },
                new User 
                { 
                    UserId = 2, 
                    RoleId = 2, 
                    FullName = "Employee User", 
                    Email = "employee@bfassystem.com", 
                    Password = "Emp123", // Should be hashed
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 10, 20)
                },
                new User 
                { 
                    UserId = 3, 
                    RoleId = 3, 
                    FullName = "Customer One", 
                    Email = "customer1@mail.com", 
                    Password = "Cust123", // Should be hashed
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 10, 20)
                }
            );

            // Seed Account Number Generator
            modelBuilder.Entity<AccountNumberGenerator>().HasData(
                new AccountNumberGenerator { Id = 1, LastNumber = 100000000000 }
            );

            // Seed Bank Account
            modelBuilder.Entity<BankAccount>().HasData(
                new BankAccount
                {
                    AccountId = 1,
                    UserId = 3,
                    AccountNumber = "BFAS-100000000001",
                    AccountType = "Savings",
                    Balance = 5000,
                    CreatedAt = new DateTime(2025, 10, 20)
                }
            );

            // Seed Billers
            modelBuilder.Entity<Biller>().HasData(
                new Biller { BillerId = 1, BillerName = "Electric Company", BillerType = "Utility", CreatedAt = DateTime.Now },
                new Biller { BillerId = 2, BillerName = "Water Company", BillerType = "Utility", CreatedAt = DateTime.Now },
                new Biller { BillerId = 3, BillerName = "Internet Provider", BillerType = "Telecom", CreatedAt = DateTime.Now }
            );

            // Seed Branches
            modelBuilder.Entity<Branch>().HasData(
                new Branch { BranchId = 1, BranchName = "Main Branch", Location = "Downtown City Center" },
                new Branch { BranchId = 2, BranchName = "North Branch", Location = "North District" }
            );
        }
    }
}
