using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("system_settings")]
    public class SystemSettings
    {
        [Key]
        [Column("setting_id")]
        public int SettingId { get; set; }

        [Required]
        [Column("setting_key")]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        [Column("setting_value")]
        [StringLength(500)]
        public string SettingValue { get; set; } = string.Empty;

        [Column("setting_type")]
        [StringLength(50)]
        public string SettingType { get; set; } = "String"; // String, Number, Decimal, Boolean

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("category")]
        [StringLength(100)]
        public string? Category { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column("updated_by")]
        public int? UpdatedBy { get; set; }

        // Navigation property
        [ForeignKey("UpdatedBy")]
        public virtual User? Updater { get; set; }
    }
}
