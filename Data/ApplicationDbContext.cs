using Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        private const string AuthorIdFk = "AuthorId";
        private const string PostIdFk = "PostId";
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Post>().Property<int>(AuthorIdFk);
            builder.Entity<Comment>().Property<int>(AuthorIdFk);
            builder.Entity<Comment>().Property<int>(PostIdFk);

            builder.Entity<Post>()
                .HasOne(x => x.Author)
                .WithMany(x => x.Posts)
                .HasForeignKey(AuthorIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Post>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Post)
                .HasForeignKey(PostIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(x => x.Comments)
                .WithOne(x => x.Author)
                .HasForeignKey(AuthorIdFk)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasOne(x => x.RefreshToken)
                .WithOne()
                .HasForeignKey<RefreshToken>()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}