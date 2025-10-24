using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("customer_profiles")]
    public class CustomerProfile
    {
        [Key]
        [Column("profile_id")]
        public int ProfileId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Column("phone")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
