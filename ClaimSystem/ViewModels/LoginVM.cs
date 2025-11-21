using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.ViewModels
{
    public class LoginVM
    {
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
