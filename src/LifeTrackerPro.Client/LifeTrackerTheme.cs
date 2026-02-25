using MudBlazor;

namespace LifeTrackerPro.Client;

public static class LifeTrackerTheme
{
    public static readonly MudTheme DarkTheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#6366F1",
            Secondary = "#8B5CF6",
            Tertiary = "#10B981",
            Info = "#3B82F6",
            Success = "#22C55E",
            Warning = "#EAB308",
            Error = "#EF4444",
            AppbarBackground = "#0c0a1a",
            Background = "#0c0a1a",
            Surface = "rgba(15,23,42,0.5)",
            DrawerBackground = "rgba(15,23,42,0.6)",
            DrawerText = "#E2E8F0",
            TextPrimary = "#F1F5F9",
            TextSecondary = "#94A3B8",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#6366F1",
            Secondary = "#8B5CF6",
            Tertiary = "#10B981",
            Info = "#3B82F6",
            Success = "#22C55E",
            Warning = "#EAB308",
            Error = "#EF4444",
            AppbarBackground = "#0c0a1a",
            Background = "#0c0a1a",
            Surface = "rgba(15,23,42,0.5)",
            DrawerBackground = "rgba(15,23,42,0.6)",
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
                FontFamily = new[] { "DM Sans", "sans-serif" },
                FontSize = "14px"
            },
            Body2 = new Body2Typography
            {
                FontFamily = new[] { "DM Sans", "sans-serif" },
                FontSize = "13px"
            },
            H4 = new H4Typography
            {
                FontFamily = new[] { "Playfair Display", "Georgia", "serif" },
                FontWeight = "700",
                LetterSpacing = "-0.02em"
            },
            H5 = new H5Typography
            {
                FontFamily = new[] { "Playfair Display", "Georgia", "serif" },
                FontWeight = "700",
                LetterSpacing = "-0.02em"
            },
            H6 = new H6Typography
            {
                FontFamily = new[] { "DM Sans", "sans-serif" },
                FontWeight = "600"
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = new[] { "DM Sans", "sans-serif" },
                FontWeight = "600"
            },
            Caption = new CaptionTypography
            {
                FontFamily = new[] { "Space Mono", "monospace" }
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "0px"
        }
    };
}
