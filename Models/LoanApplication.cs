using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("loan_applications")]
    public class LoanApplication
    {
        [Key]
        [Column("application_id")]
        public int ApplicationId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("application_number")]
        [StringLength(50)]
        public string ApplicationNumber { get; set; } = string.Empty;

        [Required]
        [Column("loan_type")]
        [StringLength(50)]
        public string LoanType { get; set; } = string.Empty; // Personal, Business, Home, Auto, Education

        [Required]
        [Column("requested_amount", TypeName = "decimal(18,2)")]
        public decimal RequestedAmount { get; set; }

        [Required]
        [Column("purpose")]
        [StringLength(1000)]
        public string Purpose { get; set; } = string.Empty;

        [Column("employment_status")]
        [StringLength(50)]
        public string? EmploymentStatus { get; set; }

        [Column("monthly_income", TypeName = "decimal(18,2)")]
        public decimal? MonthlyIncome { get; set; }

        [Column("credit_score")]
        public int? CreditScore { get; set; }

        [Column("collateral_description")]
        [StringLength(1000)]
        public string? CollateralDescription { get; set; }

        [Column("collateral_value", TypeName = "decimal(18,2)")]
        public decimal? CollateralValue { get; set; }

        [Column("term_months")]
        public int TermMonths { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Under Review, Approved, Rejected, Cancelled

        [Column("application_date")]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;

        [Column("reviewed_by")]
        public int? ReviewedBy { get; set; }

        [Column("review_date")]
        public DateTime? ReviewDate { get; set; }

        [Column("review_notes")]
        [StringLength(1000)]
        public string? ReviewNotes { get; set; }

        [Column("approved_amount", TypeName = "decimal(18,2)")]
        public decimal? ApprovedAmount { get; set; }

        [Column("interest_rate", TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
