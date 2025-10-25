using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("card_requests")]
    public class CardRequest
    {
        [Key]
        [Column("request_id")]
        public int RequestId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("card_type")]
        [StringLength(20)]
        public string CardType { get; set; } = "Debit";

        [Column("requested_limit", TypeName = "decimal(18,2)")]
        public decimal? RequestedLimit { get; set; }

        [Column("reason")]
        [StringLength(500)]
        public string? Reason { get; set; }

        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("approved_by")]
        public int? ApprovedBy { get; set; }

        [Column("approval_date")]
        public DateTime? ApprovalDate { get; set; }

        [Column("rejection_reason")]
        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("AccountId")]
        public virtual BankAccount? Account { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User? Approver { get; set; }
    }
}
