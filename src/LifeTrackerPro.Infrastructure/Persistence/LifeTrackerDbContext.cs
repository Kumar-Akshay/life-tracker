using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Infrastructure.Persistence;

public class LifeTrackerDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUser;

    public LifeTrackerDbContext(DbContextOptions<LifeTrackerDbContext> options)
        : base(options) { }

    public LifeTrackerDbContext(DbContextOptions<LifeTrackerDbContext> options, ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(LifeTrackerDbContext).Assembly);

        // Global query filters for user-scoped entities
        if (_currentUser is not null)
        {
            builder.Entity<DailyLog>().HasQueryFilter(e => e.UserId == _currentUser.UserId);
            builder.Entity<Category>().HasQueryFilter(e => e.UserId == _currentUser.UserId);
            builder.Entity<Transaction>().HasQueryFilter(e => e.UserId == _currentUser.UserId);
            builder.Entity<UserSettings>().HasQueryFilter(e => e.UserId == _currentUser.UserId);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
