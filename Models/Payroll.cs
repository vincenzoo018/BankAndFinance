using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("payroll")]
    public class Payroll
    {
        [Key]
        [Column("payroll_id")]
        public int PayrollId { get; set; }

        [Required]
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [Required]
        [Column("pay_period_start")]
        public DateTime PayPeriodStart { get; set; }

        [Required]
        [Column("pay_period_end")]
        public DateTime PayPeriodEnd { get; set; }

        [Column("pay_date")]
        public DateTime PayDate { get; set; }

        [Column("basic_salary", TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Column("overtime_hours", TypeName = "decimal(5,2)")]
        public decimal OvertimeHours { get; set; } = 0;

        [Column("overtime_pay", TypeName = "decimal(18,2)")]
        public decimal OvertimePay { get; set; } = 0;

        [Column("bonuses", TypeName = "decimal(18,2)")]
        public decimal Bonuses { get; set; } = 0;

        [Column("allowances", TypeName = "decimal(18,2)")]
        public decimal Allowances { get; set; } = 0;

        [Column("gross_pay", TypeName = "decimal(18,2)")]
        public decimal GrossPay { get; set; }

        [Column("tax_deduction", TypeName = "decimal(18,2)")]
        public decimal TaxDeduction { get; set; } = 0;

        [Column("insurance_deduction", TypeName = "decimal(18,2)")]
        public decimal InsuranceDeduction { get; set; } = 0;

        [Column("other_deductions", TypeName = "decimal(18,2)")]
        public decimal OtherDeductions { get; set; } = 0;

        [Column("total_deductions", TypeName = "decimal(18,2)")]
        public decimal TotalDeductions { get; set; }

        [Column("net_pay", TypeName = "decimal(18,2)")]
        public decimal NetPay { get; set; }

        [Column("payment_method")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Bank Transfer"; // Bank Transfer, Check, Cash

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Processed, Paid

        [Column("notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Column("processed_by")]
        public int? ProcessedBy { get; set; }

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}
