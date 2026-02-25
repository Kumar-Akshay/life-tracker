using MudBlazor;

namespace LifeTrackerPro.Client;

public static class LifeTrackerTheme
{
    public static readonly MudTheme DarkTheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#6366F1",
            Secondary = "#A855F7",
            Tertiary = "#10B981",
            Info = "#3B82F6",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            AppbarBackground = "#0c0a1a",
            Background = "#0c0a1a",
            Surface = "#1a1830",
            DrawerBackground = "#0f0d24",
            DrawerText = "#E2E8F0",
            TextPrimary = "#F1F5F9",
            TextSecondary = "#94A3B8",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#6366F1",
            Secondary = "#A855F7",
            Tertiary = "#10B981",
            Info = "#3B82F6",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            AppbarBackground = "#0c0a1a",
            Background = "#0c0a1a",
            Surface = "#1a1830",
            DrawerBackground = "#0f0d24",
            DrawerText = "#E2E8F0",
            TextPrimary = "#F1F5F9",
            TextSecondary = "#94A3B8",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "DM Sans", "Segoe UI", "Helvetica Neue", "Arial", "sans-serif" }
            },
            Body1 = new Body1Typography
            {
                FontFamily = new[] { "DM Sans", "sans-serif" }
            },
            H6 = new H6Typography
            {
                FontFamily = new[] { "DM Sans", "sans-serif" },
                FontWeight = "600"
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "260px"
        }
    };
}
