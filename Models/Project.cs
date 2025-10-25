using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("projects")]
    public class Project
    {
        [Key]
        [Column("project_id")]
        public int ProjectId { get; set; }

        [Required]
        [Column("project_code")]
        [StringLength(50)]
        public string ProjectCode { get; set; } = string.Empty;

        [Required]
        [Column("project_name")]
        [StringLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        [Column("description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Column("client_name")]
        [StringLength(200)]
        public string? ClientName { get; set; }

        [Column("project_manager_id")]
        public int? ProjectManagerId { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("budget", TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }

        [Column("actual_cost", TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; } = "Planning"; // Planning, In Progress, On Hold, Completed, Cancelled

        [Column("priority")]
        [StringLength(20)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        [Column("completion_percentage")]
        public int CompletionPercentage { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
