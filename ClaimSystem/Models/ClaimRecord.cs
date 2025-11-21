using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaimSystem.Models
{
    public class ClaimRecord
    {
        [Key]
        public int ClaimId { get; set; }

        // Identity user id (string)
        [Required]
        public string LecturerId { get; set; } = string.Empty;

        [ForeignKey("LecturerId")]
        public ApplicationUser? Lecturer { get; set; }

        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; } // HoursWorked * HourlyRate

        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, UnderReview, Approved, Rejected

        public string? UploadedFileName { get; set; }
        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    }
}
