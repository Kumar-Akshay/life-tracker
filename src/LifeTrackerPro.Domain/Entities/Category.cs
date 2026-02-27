using LifeTrackerPro.Domain.Common;

namespace LifeTrackerPro.Domain.Entities;

public class Category : BaseUserEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Letter { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366F1";
    public string? IconName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? GroupName { get; set; }
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
}
