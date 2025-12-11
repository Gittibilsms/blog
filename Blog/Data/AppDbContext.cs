using Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace Blog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<BlogPost>()
                .HasIndex(x => x.Slug).IsUnique();

            b.Entity<AdminUser>()
                .Property(a => a.AdminId)
                .ValueGeneratedOnAdd();
             

            // 1) Use a FIXED bcrypt hash string (compute once, paste literal)
            // Example placeholder below — replace with YOUR real hash string.
            const string adminHash = "$2a$12$PASTE_A_REAL_FIXED_BCRYPT_HASH_HERE";

            // 2) Use a fixed timestamp (static)
            var created = new DateTime(2025, 11, 3, 0, 0, 0, DateTimeKind.Utc);

            b.Entity<AdminUser>().HasData(new AdminUser
            {
                AdminId = 1,
                Username = "admin",
                PasswordHash = adminHash,
                IsActive = true,
                CreatedDate = created
            });
        }
    }
}
