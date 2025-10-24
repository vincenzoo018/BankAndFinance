using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("billers")]
    public class Biller
    {
        [Key]
        [Column("biller_id")]
        public int BillerId { get; set; }

        [Required]
        [Column("biller_name")]
        [StringLength(100)]
        public string BillerName { get; set; } = string.Empty;

        [Column("biller_type")]
        [StringLength(50)]
        public string BillerType { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
