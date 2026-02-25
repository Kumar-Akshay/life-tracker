using LifeTrackerPro.Domain.Common;

namespace LifeTrackerPro.Domain.Entities;

public class DailyLog : BaseUserEntity
{
    public DateOnly Date { get; set; }

    /// <summary>
    /// JSON array of hour slot assignments, e.g. ["S","S","S","S","S","S","W","W","C","W","W","W","F","W","W","W","W","C","E","N","N","P","P","S"]
    /// </summary>
    public string HourSlots { get; set; } = "[]";

    public decimal? Weight { get; set; }
    public decimal? AmountSpent { get; set; }
    public string? Highlight { get; set; }
    public int? MoodScore { get; set; }
    public int? ProductivityScore { get; set; }
    public bool IsAutoFilled { get; set; }
    public string? Notes { get; set; }
}
