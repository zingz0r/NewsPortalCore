using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsPortal.Entity;

namespace NewsPortal.Entity
{
    public class NewsPortalContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public NewsPortalContext(DbContextOptions<NewsPortalContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("Users");
        }

        public DbSet<Article> Article { get; set; }
        public DbSet<Picture> Picture { get; set; }
        public DbSet<NewsPortal.Entity.User> User { get; set; }
    }
}
