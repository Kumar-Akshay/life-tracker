using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Settings.Commands;

public record UpdateSettingsCommand : IRequest<ApiResponse<UserSettingsDto>>
{
    public string? TimeGranularity { get; init; }
    public string? ThemeMode { get; init; }
    public bool? AiAutoFillEnabled { get; init; }
    public decimal? MonthlyBudget { get; init; }
    public string? CurrencyCode { get; init; }
    public string? Timezone { get; init; }
    public bool? NotificationsEnabled { get; init; }
    public string? WeekStartDay { get; init; }
}
