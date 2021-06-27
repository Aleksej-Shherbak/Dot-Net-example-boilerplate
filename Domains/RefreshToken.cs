using System;

namespace Domains
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public User User { get; set; }
    }
}