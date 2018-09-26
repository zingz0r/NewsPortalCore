using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsPortal.Models.Entiy;

namespace NewsPortal.Models.Entiy
{
    public class NewsPortalContext : DbContext
    {
        public NewsPortalContext(DbContextOptions<NewsPortalContext> options)
               : base(options)
        {
        }
        
        public DbSet<Article> Article { get; set; }

        public DbSet<User> User { get; set; }
    }
}
