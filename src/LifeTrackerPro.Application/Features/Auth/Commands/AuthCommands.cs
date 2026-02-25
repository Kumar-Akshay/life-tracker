using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Auth.Commands;

public record RegisterCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? Timezone { get; init; }
}

public record LoginCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record RefreshTokenCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public record LogoutCommand : IRequest<ApiResponse<bool>>
{
    public string RefreshToken { get; init; } = string.Empty;
}
