namespace LifeTrackerPro.Shared.DTOs;

public class UserSettingsDto
{
    public string TimeGranularity { get; set; } = "OneHour";
    public string ThemeMode { get; set; } = "Dark";
    public bool AiAutoFillEnabled { get; set; }
    public decimal? MonthlyBudget { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string Timezone { get; set; } = "Asia/Kolkata";
    public bool NotificationsEnabled { get; set; }
    public string? WeekStartDay { get; set; }
}

public record UpdateSettingsRequestDto
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
