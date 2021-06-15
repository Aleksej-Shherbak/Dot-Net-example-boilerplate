using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.Auth
{
    public class LoginRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}