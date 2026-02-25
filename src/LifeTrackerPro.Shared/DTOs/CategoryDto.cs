namespace LifeTrackerPro.Shared.DTOs;

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Letter { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366F1";
    public string? IconName { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
    public bool IsArchived { get; init; }
}

public class CreateCategoryRequestDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Letter { get; set; } = string.Empty;
    public string Color { get; set; } = "#6366F1";
    public string? IconName { get; set; }
    public string? GroupName { get; set; }
    public string? Description { get; set; }
}

public record UpdateCategoryRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Letter { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366F1";
    public string? IconName { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
}

public record ReorderCategoriesRequestDto
{
    public List<CategoryOrderItem> Items { get; init; } = new();
}

public record CategoryOrderItem
{
    public Guid Id { get; init; }
    public int SortOrder { get; init; }
}
