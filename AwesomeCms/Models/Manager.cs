using AwesomeCms.HelperModels;
using System.ComponentModel.DataAnnotations;

namespace AwesomeCms.Models
{
    public class Manager
    {
        [Key]
        public int ManagerId { get; set; }

        [Required]
        [MaxLength(200)]
        public string ManagerName { get; set; }

        [Required]
        public ManagerType ManagerType { get; set; }

        [Required]
        public string ImageLocation { get; set; }

        [Required]
        public decimal BaseSalary { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
