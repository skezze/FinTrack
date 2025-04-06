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

            var passwordHasher = new PasswordHasher<IdentityUser>();

            var adminUser = new IdentityUser
            {
                Id = "22222222-2222-2222-2222-222222222222",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEJeySrostD/e3GSZc0ID0OaNXMTLh9RJTKTMbZal2Erq0K2r8xmn/TQ6JBR55apbew=="//Admin123!
            };

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "11111111-1111-1111-1111-111111111111",
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            builder.Entity<IdentityUser>().HasData(adminUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = "22222222-2222-2222-2222-222222222222",
                RoleId = "11111111-1111-1111-1111-111111111111"
            });
        }

    }
}