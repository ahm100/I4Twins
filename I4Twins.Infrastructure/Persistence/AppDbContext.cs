using I4Twins.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace I4Twins.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Reading> Readings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reading>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Metric)
             .HasConversion<string>()
             .HasMaxLength(20);

            // کلید یکتا برای deduplication
            entity.HasIndex(r => new { r.DeviceId, r.Metric, r.Timestamp, r.Seq })
                  .IsUnique();
        });
    }
}