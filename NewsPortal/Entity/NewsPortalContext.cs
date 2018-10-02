using Microsoft.EntityFrameworkCore;

namespace NewsPortal.Entity
{
    public class NewsPortalContext : DbContext
    {
        public NewsPortalContext(DbContextOptions<NewsPortalContext> options)
               : base(options)
        {
        }
        
        public DbSet<Article> Article { get; set; }
        public DbSet<Picture> Picture { get; set; }
        public DbSet<User> User { get; set; }
    }
}
