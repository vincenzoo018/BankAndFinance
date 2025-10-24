using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("qr_payments")]
    public class QRPayment
    {
        [Key]
        [Column("qr_id")]
        public int QRId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("qr_code")]
        [StringLength(255)]
        public string QRCode { get; set; } = string.Empty;

        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual BankAccount? Account { get; set; }

        // Calculated properties
        [NotMapped]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.Now;
    }
}
