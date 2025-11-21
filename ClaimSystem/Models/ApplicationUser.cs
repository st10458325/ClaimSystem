using Microsoft.AspNetCore.Identity;

namespace ClaimSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
