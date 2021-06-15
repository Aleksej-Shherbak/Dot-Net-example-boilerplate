using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.Auth
{
    public class Register
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "User name can not be empty.")]
        [RegularExpression(@"^[\w\d\s]{1,30}$", 
            ErrorMessage = "Allows: Lowercase and uppercase letters, digits.")]
        public string UserName { get; set; }
    }
}