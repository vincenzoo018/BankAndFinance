using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("savings_goals")]
    public class SavingsGoal
    {
        [Key]
        [Column("goal_id")]
        public int GoalId { get; set; }

        [Required]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Required]
        [Column("goal_name")]
        [StringLength(100)]
        public string GoalName { get; set; } = string.Empty;

        [Required]
        [Column("target_amount", TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Column("current_amount", TypeName = "decimal(18,2)")]
        public decimal CurrentAmount { get; set; } = 0;

        [Column("target_date")]
        public DateTime? TargetDate { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("AccountId")]
        public virtual BankAccount? Account { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal ProgressPercentage => TargetAmount > 0 
            ? (CurrentAmount / TargetAmount) * 100 
            : 0;

        [NotMapped]
        public decimal RemainingAmount => TargetAmount - CurrentAmount;

        [NotMapped]
        public int? DaysRemaining => TargetDate.HasValue 
            ? (TargetDate.Value - DateTime.Now).Days 
            : null;
    }
}
