using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("cards")]
    public class Card
    {
        [Key]
        [Column("card_id")]
        public int CardId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("card_number")]
        [StringLength(19)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [Column("card_type")]
        [StringLength(20)]
        public string CardType { get; set; } = "Debit"; // Debit, Credit

        [Column("card_status")]
        [StringLength(20)]
        public string CardStatus { get; set; } = "Active";

        [Column("cvv")]
        [StringLength(4)]
        public string? CVV { get; set; }

        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [Column("card_limit", TypeName = "decimal(18,2)")]
        public decimal? CardLimit { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual BankAccount? Account { get; set; }
    }
}
