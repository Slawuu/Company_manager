using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Nieprawidłowy format telefonu")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Data zatrudnienia jest wymagana")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Stanowisko jest wymagane")]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Wynagrodzenie musi być dodatnie")]
        public decimal Salary { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? UserId { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
