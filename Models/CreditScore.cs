using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("credit_scores")]
    public class CreditScore
    {
        [Key]
        [Column("score_id")]
        public int ScoreId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("score")]
        public int Score { get; set; } // 300-850

        [Column("score_date")]
        public DateTime ScoreDate { get; set; } = DateTime.Now;

        [Column("credit_rating")]
        [StringLength(20)]
        public string CreditRating { get; set; } = string.Empty; // Excellent, Good, Fair, Poor

        [Column("payment_history_score")]
        public int PaymentHistoryScore { get; set; }

        [Column("credit_utilization")]
        public decimal CreditUtilization { get; set; }

        [Column("credit_age_months")]
        public int CreditAgeMonths { get; set; }

        [Column("total_accounts")]
        public int TotalAccounts { get; set; }

        [Column("hard_inquiries")]
        public int HardInquiries { get; set; }

        [Column("derogatory_marks")]
        public int DerogatoryMarks { get; set; }

        [Column("total_debt", TypeName = "decimal(18,2)")]
        public decimal TotalDebt { get; set; }

        [Column("available_credit", TypeName = "decimal(18,2)")]
        public decimal AvailableCredit { get; set; }

        [Column("previous_score")]
        public int? PreviousScore { get; set; }

        [Column("score_change")]
        public int ScoreChange { get; set; } = 0;

        [Column("factors_affecting")]
        [StringLength(1000)]
        public string? FactorsAffecting { get; set; }

        [Column("recommendations")]
        [StringLength(1000)]
        public string? Recommendations { get; set; }

        [Column("next_update_date")]
        public DateTime NextUpdateDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
