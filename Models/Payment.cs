using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("biller_id")]
        public int BillerId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column("reference_number")]
        [StringLength(50)]
        public string ReferenceNumber { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("AccountId")]
        public virtual BankAccount? BankAccount { get; set; }

        [ForeignKey("BillerId")]
        public virtual Biller? Biller { get; set; }
    }
}
