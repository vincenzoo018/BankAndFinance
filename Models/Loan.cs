using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("loans")]
    public class Loan
    {
        [Key]
        [Column("loan_id")]
        public int LoanId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("loan_number")]
        [StringLength(50)]
        public string LoanNumber { get; set; } = string.Empty;

        [Required]
        [Column("loan_type")]
        [StringLength(50)]
        public string LoanType { get; set; } = string.Empty; // Personal, Business, Home, Auto, Education

        [Required]
        [Column("loan_amount", TypeName = "decimal(18,2)")]
        public decimal LoanAmount { get; set; }

        [Column("interest_rate", TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Column("term_months")]
        public int TermMonths { get; set; }

        [Column("monthly_payment", TypeName = "decimal(18,2)")]
        public decimal MonthlyPayment { get; set; }

        [Column("outstanding_balance", TypeName = "decimal(18,2)")]
        public decimal OutstandingBalance { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Paid, Defaulted, Closed

        [Column("purpose")]
        [StringLength(500)]
        public string? Purpose { get; set; }

        [Column("collateral")]
        [StringLength(500)]
        public string? Collateral { get; set; }

        [Column("approved_by")]
        public int? ApprovedBy { get; set; }

        [Column("approved_date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
