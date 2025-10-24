using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public int NotificationId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("title")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("message")]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [Column("type")]
        [StringLength(20)]
        public string? Type { get; set; } // Info, Success, Warning, Error

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
