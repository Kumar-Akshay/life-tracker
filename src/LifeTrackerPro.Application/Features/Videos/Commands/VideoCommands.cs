using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Videos.Commands;

public record CreateVideoCommand : IRequest<ApiResponse<VideoDto>>
{
    public string Title { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public List<string> Tags { get; init; } = new();
    public string? Notes { get; init; }
}

public record UpdateVideoCommand : IRequest<ApiResponse<VideoDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int ProgressPercent { get; init; }
    public string Status { get; init; } = "ToStart";
    public List<string> Tags { get; init; } = new();
    public string? Notes { get; init; }
}

public record DeleteVideoCommand(Guid Id) : IRequest<ApiResponse<bool>>;
