using MudBlazor;

namespace LifeTrackerPro.Client;

/// <summary>
/// LifeTracker Pro — Editorial Edition
/// Palette: Crail · Cloudy · Pampas · White
/// "Kinfolk magazine meets productivity"
/// </summary>
public static class LifeTrackerTheme
{
    // ── Palette constants ──
    public const string Crail = "#C15F3C";
    public const string CrailLight = "#D4795A";
    public const string CrailPale = "rgba(193,95,60,0.08)";
    public const string Cloudy = "#B1ADA1";
    public const string Pampas = "#F4F3EE";
    public const string White = "#FFFFFF";
    public const string Ink = "#2C2825";
    public const string InkSoft = "#4A4541";
    public const string InkMuted = "#7A7570";
    public const string InkFaint = "#A8A29E";
    public const string Border = "#E8E6E1";
    public const string BorderAccent = "#D4D0C8";
    public const string Success = "#5B8C51";
    public const string Warning = "#C49A2D";
    public const string Danger = "#C15050";

    // ── Category colours (12) ──
    public static readonly string[] CategoryColors = new[]
    {
        "#6B8EC7", "#C15F3C", "#C49A2D", "#B1ADA1", "#5B8C51", "#5CAFC7",
        "#4A7EC7", "#C76B8E", "#8E6BC7", "#7BA84A", "#4AC7A5", "#D4795A"
    };

    public static readonly MudTheme EditorialTheme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = Crail,
            PrimaryDarken = "#A14E30",
            PrimaryLighten = CrailLight,
            Secondary = Cloudy,
            Tertiary = "#5CAFC7",
            Info = "#6B8EC7",
            Success = Success,
            Warning = Warning,
            Error = Danger,
            AppbarBackground = White,
            AppbarText = Ink,
            Background = Pampas,
            Surface = White,
            DrawerBackground = White,
            DrawerText = InkMuted,
            TextPrimary = Ink,
            TextSecondary = InkMuted,
            TextDisabled = InkFaint,
            ActionDefault = InkMuted,
            ActionDisabled = InkFaint,
            Divider = Border,
            DividerLight = "#F0EFEA",
            LinesDefault = Border,
            TableHover = CrailPale,
            TableStriped = "rgba(0,0,0,0.015)",
        },
        PaletteDark = new PaletteDark
        {
            Primary = Crail,
            Secondary = Cloudy,
            Background = Pampas,
            Surface = White,
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Source Sans 3", "Segoe UI", "Helvetica Neue", "Arial", "sans-serif" },
                FontSize = "14px",
                LineHeight = "1.6"
            },
            H1 = new H1Typography
            {
                FontFamily = new[] { "Libre Baskerville", "Georgia", "serif" },
                FontSize = "42px",
                FontWeight = "300",
                LineHeight = "1.15"
            },
            H2 = new H2Typography
            {
                FontFamily = new[] { "Libre Baskerville", "Georgia", "serif" },
                FontSize = "34px",
                FontWeight = "400",
                LineHeight = "1.2"
            },
            H3 = new H3Typography
            {
                FontFamily = new[] { "Libre Baskerville", "Georgia", "serif" },
                FontSize = "28px",
                FontWeight = "400",
                LineHeight = "1.25"
            },
            H4 = new H4Typography
            {
                FontFamily = new[] { "Libre Baskerville", "Georgia", "serif" },
                FontSize = "22px",
                FontWeight = "700",
                LineHeight = "1.3"
            },
            H5 = new H5Typography
            {
                FontFamily = new[] { "Libre Baskerville", "Georgia", "serif" },
                FontSize = "18px",
                FontWeight = "700",
                LineHeight = "1.35"
            },
            H6 = new H6Typography
            {
                FontFamily = new[] { "Source Sans 3", "sans-serif" },
                FontSize = "15px",
                FontWeight = "600",
                LineHeight = "1.4"
            },
            Body1 = new Body1Typography
            {
                FontFamily = new[] { "Source Sans 3", "sans-serif" },
                FontSize = "14px",
                LineHeight = "1.6"
            },
            Body2 = new Body2Typography
            {
                FontFamily = new[] { "Source Sans 3", "sans-serif" },
                FontSize = "13px",
                LineHeight = "1.5"
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontFamily = new[] { "JetBrains Mono", "monospace" },
                FontSize = "11px",
                FontWeight = "700",
                LineHeight = "1.4"
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontFamily = new[] { "JetBrains Mono", "monospace" },
                FontSize = "10px",
                FontWeight = "600",
                LineHeight = "1.4"
            },
            Caption = new CaptionTypography
            {
                FontFamily = new[] { "JetBrains Mono", "monospace" },
                FontSize = "10px",
                FontWeight = "600",
                LineHeight = "1.3"
            }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "14px",
            DrawerWidthLeft = "232px"
        },
        Shadows = new Shadow
        {
            Elevation = new string[]
            {
                "none",
                "0 1px 3px rgba(44,40,37,0.04), 0 1px 2px rgba(44,40,37,0.06)",
                "0 2px 6px rgba(44,40,37,0.05), 0 1px 3px rgba(44,40,37,0.04)",
                "0 4px 12px rgba(44,40,37,0.06), 0 2px 4px rgba(44,40,37,0.04)",
                "0 6px 16px rgba(44,40,37,0.07), 0 2px 6px rgba(44,40,37,0.04)",
                "0 8px 24px rgba(44,40,37,0.08), 0 4px 8px rgba(44,40,37,0.04)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
                "0 12px 28px rgba(44,40,37,0.09), 0 4px 10px rgba(44,40,37,0.05)",
            }
        }
    };
}
