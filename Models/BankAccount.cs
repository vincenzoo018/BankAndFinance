using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("bank_accounts")]
    public class BankAccount
    {
        [Key]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("account_number")]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [Column("account_type")]
        [StringLength(20)]
        public string AccountType { get; set; } = "Savings";

        [Required]
        [Column("balance", TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Transfer> SentTransfers { get; set; } = new List<Transfer>();
        public virtual ICollection<Transfer> ReceivedTransfers { get; set; } = new List<Transfer>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
