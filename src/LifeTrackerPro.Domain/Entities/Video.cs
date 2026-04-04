using LifeTrackerPro.Domain.Common;
using LifeTrackerPro.Domain.Enums;

namespace LifeTrackerPro.Domain.Entities;

public class Video : BaseUserEntity
{
    public string Title { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int ProgressPercent { get; set; }   // 0-100
    public VideoStatus Status { get; set; } = VideoStatus.ToStart;
    public string? Tags { get; set; }           // JSON array of strings
    public string? Notes { get; set; }
}
