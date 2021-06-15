using System;
using System.Collections.Generic;

namespace Domains
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public User Author { get; set; }
        public List<Comment> Comments { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}