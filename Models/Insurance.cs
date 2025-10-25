using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("insurances")]
    public class Insurance
    {
        [Key]
        [Column("insurance_id")]
        public int InsuranceId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("policy_number")]
        [StringLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        [Column("insurance_type")]
        [StringLength(50)]
        public string InsuranceType { get; set; } = string.Empty; // Life, Health, Auto, Home, Travel

        [Required]
        [Column("policy_name")]
        [StringLength(200)]
        public string PolicyName { get; set; } = string.Empty;

        [Column("coverage_amount", TypeName = "decimal(18,2)")]
        public decimal CoverageAmount { get; set; }

        [Column("premium_amount", TypeName = "decimal(18,2)")]
        public decimal PremiumAmount { get; set; }

        [Column("premium_frequency")]
        [StringLength(20)]
        public string PremiumFrequency { get; set; } = "Monthly"; // Monthly, Quarterly, Annually

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("next_payment_date")]
        public DateTime NextPaymentDate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Claimed

        [Column("beneficiary_name")]
        [StringLength(200)]
        public string? BeneficiaryName { get; set; }

        [Column("beneficiary_relationship")]
        [StringLength(50)]
        public string? BeneficiaryRelationship { get; set; }

        [Column("total_paid", TypeName = "decimal(18,2)")]
        public decimal TotalPaid { get; set; } = 0;

        [Column("claims_filed")]
        public int ClaimsFiled { get; set; } = 0;

        [Column("last_claim_date")]
        public DateTime? LastClaimDate { get; set; }

        [Column("notes")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
