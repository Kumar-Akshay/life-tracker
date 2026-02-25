using LifeTrackerPro.Domain.Common;
using LifeTrackerPro.Domain.Enums;

namespace LifeTrackerPro.Domain.Entities;

public class UserSettings : BaseUserEntity
{
    public TimeGranularity TimeGranularity { get; set; } = TimeGranularity.OneHour;
    public ThemeMode ThemeMode { get; set; } = ThemeMode.Dark;
    public bool AiAutoFillEnabled { get; set; }
    public decimal? MonthlyBudget { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string Timezone { get; set; } = "Asia/Kolkata";
    public bool NotificationsEnabled { get; set; } = true;
    public string? WeekStartDay { get; set; } = "Monday";
}
