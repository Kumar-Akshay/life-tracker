using LifeTrackerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<DailyLog> DailyLogs { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<UserSettings> UserSettings { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
