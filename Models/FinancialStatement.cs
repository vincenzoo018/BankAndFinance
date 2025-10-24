using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("financial_statements")]
    public class FinancialStatement
    {
        [Key]
        [Column("fs_id")]
        public int FsId { get; set; }

        [Required]
        [Column("statement_type")]
        [StringLength(50)]
        public string StatementType { get; set; } = string.Empty;

        [Column("period")]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty;

        [Column("total_amount", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
