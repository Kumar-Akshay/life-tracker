using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.DailyLogs.Commands;

public record UpsertDailyLogCommand : IRequest<ApiResponse<DailyLogDto>>
{
    public DateOnly Date { get; init; }
    public List<string> HourSlots { get; init; } = new();
    public decimal? Weight { get; init; }
    public decimal? AmountSpent { get; init; }
    public string? Highlight { get; init; }
    public int? MoodScore { get; init; }
    public int? ProductivityScore { get; init; }
    public string? Notes { get; init; }
}
