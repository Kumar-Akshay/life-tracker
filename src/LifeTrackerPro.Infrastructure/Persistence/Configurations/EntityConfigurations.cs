using LifeTrackerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeTrackerPro.Infrastructure.Persistence.Configurations;

public class DailyLogConfiguration : IEntityTypeConfiguration<DailyLog>
{
    public void Configure(EntityTypeBuilder<DailyLog> builder)
    {
        builder.ToTable("daily_logs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.HourSlots).HasColumnType("jsonb").HasDefaultValue("[]");
        builder.Property(e => e.Weight).HasPrecision(5, 2);
        builder.Property(e => e.AmountSpent).HasPrecision(10, 2);
        builder.Property(e => e.Highlight).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(2000);

        builder.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Letter).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Color).HasMaxLength(20).IsRequired();
        builder.Property(e => e.IconName).HasMaxLength(50);
        builder.Property(e => e.GroupName).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => new { e.UserId, e.Code }).IsUnique();
    }
}

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Amount).HasPrecision(12, 2);
        builder.Property(e => e.CategoryTag).HasMaxLength(50);
        builder.Property(e => e.CurrencyCode).HasMaxLength(10).HasDefaultValue("INR");

        builder.HasIndex(e => new { e.UserId, e.Date });
    }
}

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.ToTable("user_settings");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CurrencyCode).HasMaxLength(10).HasDefaultValue("INR");
        builder.Property(e => e.Timezone).HasMaxLength(100).HasDefaultValue("Asia/Kolkata");
        builder.Property(e => e.WeekStartDay).HasMaxLength(20);
        builder.Property(e => e.MonthlyBudget).HasPrecision(12, 2);

        builder.HasIndex(e => e.UserId).IsUnique();
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token).HasMaxLength(500).IsRequired();
        builder.Property(e => e.DeviceInfo).HasMaxLength(500);
        builder.Property(e => e.ReplacedByToken).HasMaxLength(500);

        builder.HasIndex(e => e.Token).IsUnique();
        builder.HasIndex(e => e.UserId);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Action).HasMaxLength(100).IsRequired();
        builder.Property(e => e.EntityType).HasMaxLength(100);
        builder.Property(e => e.EntityId).HasMaxLength(100);
        builder.Property(e => e.IpAddress).HasMaxLength(50);
        builder.Property(e => e.OldValues).HasColumnType("jsonb");
        builder.Property(e => e.NewValues).HasColumnType("jsonb");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Timestamp);
    }
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.FullName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Timezone).HasMaxLength(100).HasDefaultValue("Asia/Kolkata");
        builder.Property(e => e.AvatarUrl).HasMaxLength(500);

        builder.HasMany(u => u.DailyLogs).WithOne().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Categories).WithOne().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Transactions).WithOne().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(u => u.Settings).WithOne().HasForeignKey<UserSettings>(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.RefreshTokens).WithOne().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
