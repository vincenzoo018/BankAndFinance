using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("beneficiaries")]
    public class Beneficiary
    {
        [Key]
        [Column("beneficiary_id")]
        public int BeneficiaryId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("beneficiary_name")]
        [StringLength(100)]
        public string BeneficiaryName { get; set; } = string.Empty;

        [Required]
        [Column("account_number")]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Column("bank_name")]
        [StringLength(100)]
        public string? BankName { get; set; }

        [Column("nickname")]
        [StringLength(50)]
        public string? Nickname { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
