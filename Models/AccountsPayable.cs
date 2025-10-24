using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("accounts_payable")]
    public class AccountsPayable
    {
        [Key]
        [Column("ap_id")]
        public int ApId { get; set; }

        [Required]
        [Column("payee_name")]
        [StringLength(100)]
        public string PayeeName { get; set; } = string.Empty;

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
    }
}
