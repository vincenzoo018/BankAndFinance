using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("statements")]
    public class Statement
    {
        [Key]
        [Column("statement_id")]
        public int StatementId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("statement_number")]
        [StringLength(50)]
        public string StatementNumber { get; set; } = string.Empty;

        [Column("statement_type")]
        [StringLength(50)]
        public string StatementType { get; set; } = "Monthly"; // Monthly, Quarterly, Annual, Custom

        [Column("period_start")]
        public DateTime PeriodStart { get; set; }

        [Column("period_end")]
        public DateTime PeriodEnd { get; set; }

        [Column("opening_balance", TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; }

        [Column("closing_balance", TypeName = "decimal(18,2)")]
        public decimal ClosingBalance { get; set; }

        [Column("total_deposits", TypeName = "decimal(18,2)")]
        public decimal TotalDeposits { get; set; }

        [Column("total_withdrawals", TypeName = "decimal(18,2)")]
        public decimal TotalWithdrawals { get; set; }

        [Column("transaction_count")]
        public int TransactionCount { get; set; }

        [Column("interest_earned", TypeName = "decimal(18,2)")]
        public decimal InterestEarned { get; set; } = 0;

        [Column("fees_charged", TypeName = "decimal(18,2)")]
        public decimal FeesCharged { get; set; } = 0;

        [Column("generated_date")]
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        [Column("file_path")]
        [StringLength(500)]
        public string? FilePath { get; set; }

        [Column("is_downloaded")]
        public bool IsDownloaded { get; set; } = false;

        [Column("download_count")]
        public int DownloadCount { get; set; } = 0;

        [Column("last_download_date")]
        public DateTime? LastDownloadDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AccountId")]
        public virtual BankAccount? BankAccount { get; set; }
    }
}
