using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("trial_balance")]
    public class TrialBalance
    {
        [Key]
        [Column("trial_id")]
        public int TrialId { get; set; }

        [Required]
        [Column("account_name")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Column("debit", TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0;

        [Column("credit", TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0;

        [Column("period")]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty;
    }
}
