using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("investments")]
    public class Investment
    {
        [Key]
        [Column("investment_id")]
        public int InvestmentId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("investment_number")]
        [StringLength(50)]
        public string InvestmentNumber { get; set; } = string.Empty;

        [Required]
        [Column("investment_type")]
        [StringLength(50)]
        public string InvestmentType { get; set; } = string.Empty; // Mutual Fund, Stocks, Bonds, Fixed Deposit, Crypto

        [Required]
        [Column("investment_name")]
        [StringLength(200)]
        public string InvestmentName { get; set; } = string.Empty;

        [Column("principal_amount", TypeName = "decimal(18,2)")]
        public decimal PrincipalAmount { get; set; }

        [Column("current_value", TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; }

        [Column("expected_return_rate", TypeName = "decimal(5,2)")]
        public decimal ExpectedReturnRate { get; set; }

        [Column("actual_return_rate", TypeName = "decimal(5,2)")]
        public decimal? ActualReturnRate { get; set; }

        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        [Column("maturity_date")]
        public DateTime? MaturityDate { get; set; }

        [Column("risk_level")]
        [StringLength(20)]
        public string RiskLevel { get; set; } = "Medium"; // Low, Medium, High

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Matured, Redeemed, Closed

        [Column("dividends_earned", TypeName = "decimal(18,2)")]
        public decimal DividendsEarned { get; set; } = 0;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
