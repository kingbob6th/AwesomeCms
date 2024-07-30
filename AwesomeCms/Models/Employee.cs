using System.ComponentModel.DataAnnotations;

namespace AwesomeCms.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EmployeeName { get; set; }

        [Required]
        public decimal Salary { get; set; }
    }
}
