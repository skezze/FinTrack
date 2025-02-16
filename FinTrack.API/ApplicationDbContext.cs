using FinTrack.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<MonobankTransaction> MonobankTransactions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminRole = new IdentityRole
            {
                Id = adminRoleId.ToString(),
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var adminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var adminUser = new IdentityUser
            {
                Id = adminUserId.ToString(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            var adminUserRole = new IdentityUserRole<string>
            {
                UserId = adminUserId.ToString(),
                RoleId = adminRoleId.ToString()
            };

            builder.Entity<IdentityRole>().HasData(adminRole);
            builder.Entity<IdentityUser>().HasData(adminUser);
            builder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);
        }

    }
}