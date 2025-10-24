using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("rewards")]
    public class Reward
    {
        [Key]
        [Column("reward_id")]
        public int RewardId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("points")]
        public int Points { get; set; } = 0;

        [Column("tier")]
        [StringLength(20)]
        public string Tier { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum

        [Column("cashback_earned", TypeName = "decimal(18,2)")]
        public decimal CashbackEarned { get; set; } = 0;

        [Column("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Tier thresholds
        [NotMapped]
        public int NextTierPoints => Tier switch
        {
            "Bronze" => 1000,
            "Silver" => 5000,
            "Gold" => 10000,
            "Platinum" => int.MaxValue,
            _ => 1000
        };

        [NotMapped]
        public int PointsToNextTier => Math.Max(0, NextTierPoints - Points);

        [NotMapped]
        public decimal CashbackRate => Tier switch
        {
            "Bronze" => 0.01m,    // 1%
            "Silver" => 0.015m,   // 1.5%
            "Gold" => 0.02m,      // 2%
            "Platinum" => 0.03m,  // 3%
            _ => 0.01m
        };
    }
}
