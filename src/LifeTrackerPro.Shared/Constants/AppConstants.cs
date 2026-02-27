namespace LifeTrackerPro.Shared.Constants;

public static class AppConstants
{
    public const string AppName = "LifeTracker Pro";
    public const string DefaultTimezone = "UTC";
    public const string DefaultCurrency = "USD";
    public const int MaxHourSlots = 24;
    public const int MaxMoodScore = 5;
    public const int MinMoodScore = 1;
    public const int MaxProductivityScore = 10;
    public const int MinProductivityScore = 1;
}

public static class AuthConstants
{
    public const int AccessTokenExpiryMinutes = 15;
    public const int RefreshTokenExpiryDays = 7;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
}

public static class ApiRoutes
{
    public const string Auth = "/api/auth";
    public const string DailyLogs = "/api/daily-logs";
    public const string Categories = "/api/categories";
    public const string Dashboard = "/api/dashboard";
    public const string Settings = "/api/settings";
    public const string Profile = "/api/profile";
    public const string Sessions = "/api/sessions";
    public const string Import = "/api/import";
}
