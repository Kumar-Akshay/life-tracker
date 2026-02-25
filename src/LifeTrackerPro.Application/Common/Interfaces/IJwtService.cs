using LifeTrackerPro.Domain.Entities;
using System.Security.Claims;

namespace LifeTrackerPro.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateAccessToken(string token);
}
