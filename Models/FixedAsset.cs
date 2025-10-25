using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("fixed_assets")]
    public class FixedAsset
    {
        [Key]
        [Column("asset_id")]
        public int AssetId { get; set; }

        [Required]
        [Column("asset_code")]
        [StringLength(50)]
        public string AssetCode { get; set; } = string.Empty;

        [Required]
        [Column("asset_name")]
        [StringLength(200)]
        public string AssetName { get; set; } = string.Empty;

        [Column("asset_type")]
        [StringLength(100)]
        public string AssetType { get; set; } = string.Empty; // Building, Equipment, Vehicle, Furniture, IT

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        [Column("purchase_cost", TypeName = "decimal(18,2)")]
        public decimal PurchaseCost { get; set; }

        [Column("salvage_value", TypeName = "decimal(18,2)")]
        public decimal SalvageValue { get; set; } = 0;

        [Column("useful_life_years")]
        public int UsefulLifeYears { get; set; }

        [Column("depreciation_method")]
        [StringLength(50)]
        public string DepreciationMethod { get; set; } = "Straight-Line"; // Straight-Line, Declining Balance

        [Column("accumulated_depreciation", TypeName = "decimal(18,2)")]
        public decimal AccumulatedDepreciation { get; set; } = 0;

        [Column("book_value", TypeName = "decimal(18,2)")]
        public decimal BookValue { get; set; }

        [Column("location")]
        [StringLength(200)]
        public string? Location { get; set; }

        [Column("assigned_to")]
        public int? AssignedTo { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Under Maintenance, Disposed, Sold

        [Column("maintenance_schedule")]
        [StringLength(100)]
        public string? MaintenanceSchedule { get; set; }

        [Column("last_maintenance_date")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
