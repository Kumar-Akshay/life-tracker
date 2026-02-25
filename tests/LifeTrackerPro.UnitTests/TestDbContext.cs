using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.UnitTests;

/// <summary>
/// Lightweight EF context for unit testing without Identity overhead.
/// </summary>
public class TestDbContext : DbContext, IApplicationDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Minimal config for InMemory — no Identity tables
        modelBuilder.Entity<DailyLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Date);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<UserSettings>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
        });
    }
}
