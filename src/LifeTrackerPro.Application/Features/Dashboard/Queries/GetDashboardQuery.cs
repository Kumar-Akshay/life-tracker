using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Dashboard.Queries;

public record GetDashboardQuery : IRequest<ApiResponse<DashboardDto>>
{
    public string Period { get; init; } = "week"; // week, month, year
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}
