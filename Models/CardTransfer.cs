using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("card_transfers")]
    public class CardTransfer
    {
        [Key]
        [Column("transfer_id")]
        public int TransferId { get; set; }

        [Required]
        [Column("from_card_id")]
        public int FromCardId { get; set; }

        [Required]
        [Column("to_card_id")]
        public int ToCardId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Completed";

        [Column("transfer_date")]
        public DateTime TransferDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("FromCardId")]
        public virtual Card? FromCard { get; set; }

        [ForeignKey("ToCardId")]
        public virtual Card? ToCard { get; set; }
    }
}
