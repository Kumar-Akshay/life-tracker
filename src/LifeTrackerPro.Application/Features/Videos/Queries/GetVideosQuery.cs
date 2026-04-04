using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Videos.Queries;

public record GetVideosQuery : IRequest<ApiResponse<List<VideoDto>>>
{
    public string? Status { get; init; }   // optional filter
}
