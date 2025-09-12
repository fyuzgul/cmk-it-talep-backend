using System.ComponentModel.DataAnnotations;

namespace CMKITTalep.API.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
