using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAndFinance.Models
{
    [Table("account_number_generator")]
    public class AccountNumberGenerator
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("last_number")]
        public long LastNumber { get; set; } = 100000000000;
    }
}
