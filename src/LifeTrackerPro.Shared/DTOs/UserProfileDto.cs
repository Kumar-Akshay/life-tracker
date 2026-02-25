namespace LifeTrackerPro.Shared.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Timezone { get; set; } = "Asia/Kolkata";
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record UpdateProfileRequestDto
{
    public string FullName { get; init; } = string.Empty;
    public string? Timezone { get; init; }
    public string? AvatarUrl { get; init; }
}
