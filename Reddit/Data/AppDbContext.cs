using Microsoft.EntityFrameworkCore;
using Reddit.Model;

namespace Reddit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Comment> Comments{ get; set; }
        public DbSet<Post> Posts{ get; set; }
    }
}
