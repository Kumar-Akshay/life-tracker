namespace LifeTrackerPro.Shared.DTOs;

public record DailyLogDto
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public List<string> HourSlots { get; init; } = new();
    public decimal? Weight { get; init; }
    public decimal? AmountSpent { get; init; }
    public string? Highlight { get; init; }
    public int? MoodScore { get; init; }
    public int? ProductivityScore { get; init; }
    public bool IsAutoFilled { get; init; }
    public string? Notes { get; init; }
}

public record UpsertDailyLogRequestDto
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
