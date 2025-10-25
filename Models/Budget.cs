using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("budgets")]
    public class Budget
    {
        [Key]
        [Column("budget_id")]
        public int BudgetId { get; set; }

        [Required]
        [Column("department")]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Operational, Capital, Marketing, etc.

        [Column("fiscal_year")]
        public int FiscalYear { get; set; }

        [Column("quarter")]
        public int? Quarter { get; set; } // 1, 2, 3, 4

        [Column("allocated_amount", TypeName = "decimal(18,2)")]
        public decimal AllocatedAmount { get; set; }

        [Column("spent_amount", TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; } = 0;

        [Column("remaining_amount", TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Exceeded, Completed

        [Column("notes")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Column("created_by")]
        public int CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
