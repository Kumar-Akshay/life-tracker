using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class SettingsService
{
    private readonly HttpClient _http;

    public SettingsService(HttpClient http) => _http = http;

    public async Task<UserSettingsDto?> GetAsync()
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<UserSettingsDto>>("/api/settings");
        return result?.Data;
    }

    public async Task<UserSettingsDto?> UpdateAsync(UpdateSettingsRequestDto request)
    {
        var response = await _http.PutAsJsonAsync("/api/settings", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserSettingsDto>>();
        return result?.Data;
    }

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<UserProfileDto>>("/api/profile");
        return result?.Data;
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(UpdateProfileRequestDto request)
    {
        var response = await _http.PutAsJsonAsync("/api/profile", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserProfileDto>>();
        return result?.Data;
    }
}
