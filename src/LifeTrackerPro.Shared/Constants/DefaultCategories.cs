namespace LifeTrackerPro.Shared.Constants;

public static class DefaultCategories
{
    public static readonly List<DefaultCategoryDef> All = new()
    {
        new("sleep",        "Sleep",         "S", "#6366F1", "Essentials",   1),
        new("work",         "Work",          "W", "#F59E0B", "Productivity", 2),
        new("exercise",     "Exercise",      "E", "#10B981", "Health",       3),
        new("commute",      "Commute",       "C", "#8B5CF6", "Essentials",   4),
        new("chores",       "Chores",        "H", "#EC4899", "Essentials",   5),
        new("social",       "Social",        "O", "#F97316", "Relationships",6),
        new("learning",     "Learning",      "L", "#3B82F6", "Productivity", 7),
        new("entertainment","Entertainment", "N", "#EF4444", "Leisure",      8),
        new("personal",     "Personal",      "P", "#14B8A6", "Wellness",     9),
        new("food",         "Food",          "F", "#A855F7", "Essentials",  10),
        new("grooming",     "Grooming",      "G", "#06B6D4", "Essentials",  11),
        new("idle",         "Idle",          "I", "#6B7280", "Other",       12),
    };

    public record DefaultCategoryDef(
        string Code,
        string Name,
        string Letter,
        string Color,
        string GroupName,
        int SortOrder
    );
}
