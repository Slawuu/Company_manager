using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi składać się z 9 cyfr.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Data zatrudnienia jest wymagana")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Stanowisko jest wymagane")]
        [StringLength(100)]
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Wynagrodzenie musi być dodatnie")]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{8,}$", ErrorMessage = "Hasło musi mieć min. 8 znaków, 1 wielką literę i 1 znak specjalny")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rola jest wymagana")]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Employee";
    }
}
