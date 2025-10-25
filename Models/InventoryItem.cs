using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("inventory_items")]
    public class InventoryItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [Column("item_code")]
        [StringLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [Column("item_name")]
        [StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price", TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column("reorder_level")]
        public int ReorderLevel { get; set; } = 10;

        [Column("supplier")]
        [StringLength(200)]
        public string? Supplier { get; set; }

        [Column("warehouse_location")]
        [StringLength(100)]
        public string? WarehouseLocation { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Discontinued, Out of Stock

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
