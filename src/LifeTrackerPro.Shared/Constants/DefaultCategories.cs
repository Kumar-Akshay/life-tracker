namespace LifeTrackerPro.Shared.Constants;

public static class DefaultCategories
{
    /// <summary>
    /// Hardcoded fallback used when configuration is not available (e.g. unit tests).
    /// At runtime, prefer injecting <see cref="DefaultCategoriesOptions"/> via IOptions.
    /// </summary>
    public static readonly List<DefaultCategoryDef> All = new()
    {
        new() { Code = "sleep",         Name = "Sleep",         Letter = "S", Color = "#6366F1", GroupName = "Essentials",    SortOrder = 1  },
        new() { Code = "work",          Name = "Work",          Letter = "W", Color = "#F59E0B", GroupName = "Productivity",  SortOrder = 2  },
        new() { Code = "exercise",      Name = "Exercise",      Letter = "E", Color = "#10B981", GroupName = "Health",        SortOrder = 3  },
        new() { Code = "commute",       Name = "Commute",       Letter = "C", Color = "#8B5CF6", GroupName = "Essentials",    SortOrder = 4  },
        new() { Code = "chores",        Name = "Chores",        Letter = "H", Color = "#EC4899", GroupName = "Essentials",    SortOrder = 5  },
        new() { Code = "social",        Name = "Social",        Letter = "O", Color = "#F97316", GroupName = "Relationships", SortOrder = 6  },
        new() { Code = "learning",      Name = "Learning",      Letter = "L", Color = "#3B82F6", GroupName = "Productivity",  SortOrder = 7  },
        new() { Code = "entertainment", Name = "Entertainment", Letter = "N", Color = "#EF4444", GroupName = "Leisure",       SortOrder = 8  },
        new() { Code = "personal",      Name = "Personal",      Letter = "P", Color = "#14B8A6", GroupName = "Wellness",      SortOrder = 9  },
        new() { Code = "food",          Name = "Food",          Letter = "F", Color = "#A855F7", GroupName = "Essentials",    SortOrder = 10 },
        new() { Code = "grooming",      Name = "Grooming",      Letter = "G", Color = "#06B6D4", GroupName = "Essentials",    SortOrder = 11 },
        new() { Code = "idle",          Name = "Idle",          Letter = "I", Color = "#6B7280", GroupName = "Other",         SortOrder = 12 },
    };
}

/// <summary>
/// A single default-category definition. Properties are settable for configuration binding.
/// </summary>
public class DefaultCategoryDef
{
    public string Code      { get; set; } = string.Empty;
    public string Name      { get; set; } = string.Empty;
    public string Letter    { get; set; } = string.Empty;
    public string Color     { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int    SortOrder { get; set; }
}

/// <summary>
/// Options object populated from the "DefaultCategories" configuration section.
/// </summary>
public class DefaultCategoriesOptions
{
    public const string SectionName = "DefaultCategories";

    public List<DefaultCategoryDef> Categories { get; set; } = new List<DefaultCategoryDef>(DefaultCategories.All);
}
