using Microsoft.EntityFrameworkCore;
using Reddit.Model;

namespace Reddit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Community> Communities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring User and Post relationship (One-to-Many)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(p => p.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuring User and Comment relationship (One-to-Many)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuring Post and Comment relationship (One-to-Many)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(u => u.Author)
                .HasForeignKey(u => u.AuthorId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(u => u.Author)
                .HasForeignKey(u => u.AuthorId);
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Community>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.OwnedCommunities)  // Assuming the owner does not have a collection of owned communities
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading deletes if needed

            base.OnModelCreating(modelBuilder);
        }
    }
}
