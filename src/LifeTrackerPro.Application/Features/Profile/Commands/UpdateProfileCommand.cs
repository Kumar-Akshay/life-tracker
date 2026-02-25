using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Profile.Commands;

public record UpdateProfileCommand : IRequest<ApiResponse<UserProfileDto>>
{
    public string FullName { get; init; } = string.Empty;
    public string? Timezone { get; init; }
    public string? AvatarUrl { get; init; }
}
