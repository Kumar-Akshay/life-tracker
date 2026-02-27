using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Infrastructure.Persistence;
using LifeTrackerPro.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LifeTrackerPro.Infrastructure.Utils;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database — use SQLite for development, PostgreSQL for production
        var usePostgres = !string.IsNullOrEmpty(configuration.GetConnectionString("DefaultConnection")) && configuration.GetConnectionString("DefaultConnection")!.Contains("Host=");

        services.AddDbContext<LifeTrackerDbContext>(options =>
        {
            if (usePostgres)
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(LifeTrackerDbContext).Assembly.FullName));
            }
            else
            {
                var sqlitePath = configuration.GetConnectionString("DefaultConnection")
                    ?? "Data Source=lifetrackerpro.db";
                options.UseSqlite(sqlitePath);
            }
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<LifeTrackerDbContext>());

        // Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<LifeTrackerDbContext>()
        .AddDefaultTokenProviders();

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IEncryptionService>(_ =>
        {
            var encryptionKey = configuration["Encryption:Key"];
            if (string.IsNullOrWhiteSpace(encryptionKey))
                throw new InvalidOperationException(
                    "Encryption:Key is not configured. Set it in appsettings or environment variables before starting the application.");
            return new EncryptionService(encryptionKey);
        });

        return services;
    }
}
