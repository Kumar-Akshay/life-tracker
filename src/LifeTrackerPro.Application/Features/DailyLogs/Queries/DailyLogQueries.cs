using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.DailyLogs.Queries;

public record GetDailyLogQuery(DateOnly Date) : IRequest<ApiResponse<DailyLogDto>>;

public record GetDailyLogsQuery(DateOnly From, DateOnly To) : IRequest<ApiResponse<List<DailyLogDto>>>;
