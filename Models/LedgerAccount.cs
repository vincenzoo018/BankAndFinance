using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("ledger_accounts")]
    public class LedgerAccount
    {
        [Key]
        [Column("ledger_id")]
        public int LedgerId { get; set; }

        [Required]
        [Column("account_name")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Column("balance", TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
