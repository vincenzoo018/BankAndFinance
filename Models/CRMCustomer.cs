using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("crm_customers")]
    public class CRMCustomer
    {
        [Key]
        [Column("crm_customer_id")]
        public int CRMCustomerId { get; set; }

        [Required]
        [Column("customer_name")]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("company")]
        [StringLength(200)]
        public string? Company { get; set; }

        [Column("industry")]
        [StringLength(100)]
        public string? Industry { get; set; }

        [Column("lead_source")]
        [StringLength(100)]
        public string? LeadSource { get; set; }

        [Column("lead_status")]
        [StringLength(50)]
        public string LeadStatus { get; set; } = "New"; // New, Contacted, Qualified, Converted, Lost

        [Column("deal_value", TypeName = "decimal(18,2)")]
        public decimal? DealValue { get; set; }

        [Column("assigned_to")]
        public int? AssignedTo { get; set; }

        [Column("notes")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
