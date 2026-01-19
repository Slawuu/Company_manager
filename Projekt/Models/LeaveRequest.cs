using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Typ urlopu jest wymagany")]
        [StringLength(50)]
        public string LeaveType { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Reason { get; set; }

        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

        public string? ApprovedByUserId { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Comments { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;
    }

    public enum LeaveRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
