namespace LifeTrackerPro.Shared.DTOs;

public record BookDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public string Status { get; init; } = "ToStart";
    public int Rating { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? FinishDate { get; init; }
    public string? Notes { get; init; }
    public string Color { get; init; } = "#6366F1";
    public int ReadPercent => TotalPages > 0 ? (int)Math.Round(CurrentPage * 100.0 / TotalPages) : 0;
}

public class CreateBookRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public string? Color { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBookRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string Status { get; set; } = "ToStart";
    public int Rating { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? FinishDate { get; set; }
    public string? Notes { get; set; }
    public string Color { get; set; } = "#6366F1";
}
