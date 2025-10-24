using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("transfers")]
    public class Transfer
    {
        [Key]
        [Column("transfer_id")]
        public int TransferId { get; set; }

        [Required]
        [Column("sender_account_id")]
        public int SenderAccountId { get; set; }

        [Required]
        [Column("receiver_account_id")]
        public int ReceiverAccountId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("transfer_date")]
        public DateTime TransferDate { get; set; } = DateTime.Now;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("SenderAccountId")]
        public virtual BankAccount? SenderAccount { get; set; }

        [ForeignKey("ReceiverAccountId")]
        public virtual BankAccount? ReceiverAccount { get; set; }
    }
}
