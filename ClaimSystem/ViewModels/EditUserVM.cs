using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.ViewModels
{
    public class EditUserVM
    {
        public string Id { get; set; } = "";
        [Required] public string? FullName { get; set; } = "";
        [Required, EmailAddress] public string? Email { get; set; } = "";
    }
}
