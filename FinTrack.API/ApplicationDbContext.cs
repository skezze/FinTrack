using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    
}