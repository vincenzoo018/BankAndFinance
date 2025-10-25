using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("scheduled_payments")]
    public class ScheduledPayment
    {
        [Key]
        [Column("schedule_id")]
        public int ScheduleId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("card_id")]
        public int CardId { get; set; }

        [Required]
        [Column("biller_id")]
        public int BillerId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column("frequency")]
        [StringLength(20)]
        public string Frequency { get; set; } = "Monthly"; // Daily, Weekly, Monthly, Yearly

        [Column("next_payment_date")]
        public DateTime NextPaymentDate { get; set; }

        [Column("last_payment_date")]
        public DateTime? LastPaymentDate { get; set; }

        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Paused, Cancelled

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("CardId")]
        public virtual Card? Card { get; set; }

        [ForeignKey("BillerId")]
        public virtual Biller? Biller { get; set; }
    }
}
