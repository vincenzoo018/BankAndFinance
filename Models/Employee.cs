using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("employee_number")]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        [Column("department")]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Column("position")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [Column("salary", TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Column("employment_status")]
        [StringLength(20)]
        public string EmploymentStatus { get; set; } = "Active"; // Active, Inactive, Terminated

        [Column("manager_id")]
        public int? ManagerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
