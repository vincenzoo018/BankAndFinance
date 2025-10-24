using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("journal_entry_lines")]
    public class JournalEntryLine
    {
        [Key]
        [Column("line_id")]
        public int LineId { get; set; }

        [Required]
        [Column("journal_id")]
        public int JournalId { get; set; }

        [Required]
        [Column("account_name")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Column("debit", TypeName = "decimal(18,2)")]
        public decimal Debit { get; set; } = 0;

        [Column("credit", TypeName = "decimal(18,2)")]
        public decimal Credit { get; set; } = 0;

        // Navigation properties
        [ForeignKey("JournalId")]
        public virtual JournalEntry? JournalEntry { get; set; }
    }
}
