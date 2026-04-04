using LifeTrackerPro.Domain.Common;
using LifeTrackerPro.Domain.Enums;

namespace LifeTrackerPro.Domain.Entities;

public class Book : BaseUserEntity
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public BookStatus Status { get; set; } = BookStatus.ToStart;
    public int Rating { get; set; }          // 0-5 stars
    public DateOnly? StartDate { get; set; }
    public DateOnly? FinishDate { get; set; }
    public string? Notes { get; set; }
    public string Color { get; set; } = "#6366F1";
}
