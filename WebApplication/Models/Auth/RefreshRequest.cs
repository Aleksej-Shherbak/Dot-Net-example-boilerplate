using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models.Auth
{
    public class RefreshRequest
    {
        [Required]
        public string Token { get; set; }
    }
}