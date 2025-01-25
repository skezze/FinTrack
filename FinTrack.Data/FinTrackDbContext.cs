using Microsoft.EntityFrameworkCore;

namespace FinTrack.Data
{
    public class FinTrackDbContext : DbContext
    {
        public FinTrackDbContext(DbContextOptions<FinTrackDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Description).HasMaxLength(500);
            });
        }
    }
}
