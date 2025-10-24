using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("journal_entries")]
    public class JournalEntry
    {
        [Key]
        [Column("journal_id")]
        public int JournalId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column("reference_number")]
        [StringLength(50)]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required]
        [Column("created_by")]
        public int CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public virtual User? Creator { get; set; }
        
        public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
    }
}
