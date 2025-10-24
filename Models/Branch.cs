using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("branch")]
    public class Branch
    {
        [Key]
        [Column("branch_id")]
        public int BranchId { get; set; }

        [Required]
        [Column("branch_name")]
        [StringLength(100)]
        public string BranchName { get; set; } = string.Empty;

        [Column("location")]
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;
    }
}
