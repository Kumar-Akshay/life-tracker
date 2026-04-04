namespace LifeTrackerPro.Shared.DTOs;

public record VideoDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int ProgressPercent { get; init; }
    public string Status { get; init; } = "ToStart";
    public List<string> Tags { get; init; } = new();
    public string? Notes { get; init; }
    public string DurationFormatted => DurationMinutes >= 60
        ? $"{DurationMinutes / 60}h {DurationMinutes % 60}m"
        : $"{DurationMinutes}m";
}

public class CreateVideoRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Notes { get; set; }
}

public class UpdateVideoRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int ProgressPercent { get; set; }
    public string Status { get; set; } = "ToStart";
    public List<string> Tags { get; set; } = new();
    public string? Notes { get; set; }
}
