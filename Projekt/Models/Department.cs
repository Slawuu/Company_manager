using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa działu jest wymagana")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? ManagerId { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
