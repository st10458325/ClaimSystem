namespace ClaimSystem.ViewModels
{
    public class AdminClaimVM
    {
        public string? LecturerName { get; set; }
        public string? LecturerEmail { get; set; }

        public int ClaimId { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string UploadedFileName { get; set; }
    }
}
