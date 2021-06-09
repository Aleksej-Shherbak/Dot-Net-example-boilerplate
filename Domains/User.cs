using Microsoft.AspNetCore.Identity;

namespace Domains
{
    public class User : IdentityUser
    {
        public int Age { get; set; }
    }
}