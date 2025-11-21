namespace ClaimSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PasswordHash { get; set; } // keep simple - you can replace with Identity later
        public string Role { get; set; } = "Lecturer"; // Admin, Coordinator, Lecturer

        public ICollection<ClaimRecord>? Claims { get; set; }
    }

}
