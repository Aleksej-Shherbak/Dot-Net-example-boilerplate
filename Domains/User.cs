using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domains
{
    public class User : IdentityUser<int>
    {
        public int Age { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}