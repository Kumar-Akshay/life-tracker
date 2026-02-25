namespace LifeTrackerPro.Shared.DTOs;

public record DashboardDto
{
    public int TotalLoggedHours { get; init; }
    public decimal TotalSpent { get; init; }
    public int CurrentStreak { get; init; }
    public int DaysLogged { get; init; }
    public List<CategoryHoursDto> HoursPerCategory { get; init; } = new();
    public List<DailyBreakdownDto> DailyBreakdown { get; init; } = new();
    public SpendSummaryDto SpendSummary { get; init; } = new();
}

public record CategoryHoursDto
{
    public string CategoryCode { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public int Hours { get; init; }
    public double Percentage { get; init; }
}

public record DailyBreakdownDto
{
    public DateOnly Date { get; init; }
    public Dictionary<string, int> CategoryHours { get; init; } = new();
}

public record SpendSummaryDto
{
    public decimal TotalSpent { get; init; }
    public decimal AverageDaily { get; init; }
    public decimal? BudgetRemaining { get; init; }
    public string CurrencyCode { get; init; } = "INR";
}
