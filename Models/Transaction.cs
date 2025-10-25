using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("transaction_type")]
        [StringLength(50)]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Column("status")]
        [StringLength(20)]
        public string? Status { get; set; } = "Pending";

        [Column("reference_number")]
        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        [Column("payment_mode")]
        [StringLength(50)]
        public string? PaymentMode { get; set; } = "Cash";

        [Column("qr_code")]
        [StringLength(255)]
        public string? QRCode { get; set; }

        // Navigation properties
        [ForeignKey("AccountId")]
        public virtual BankAccount? BankAccount { get; set; }
    }
}
