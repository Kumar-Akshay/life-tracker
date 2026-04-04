using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class VideoService
{
    private readonly HttpClient _http;

    public VideoService(HttpClient http) => _http = http;

    public async Task<List<VideoDto>> GetAllAsync(string? status = null)
    {
        var url = "/api/videos" + (status is not null ? $"?status={status}" : "");
        var result = await _http.GetFromJsonAsync<ApiResponse<List<VideoDto>>>(url);
        return result?.Data ?? new();
    }

    public async Task<VideoDto?> CreateAsync(CreateVideoRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/videos", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<VideoDto>>();
        return result?.Data;
    }

    public async Task<VideoDto?> UpdateAsync(Guid id, UpdateVideoRequestDto request)
    {
        var response = await _http.PutAsJsonAsync($"/api/videos/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<VideoDto>>();
        return result?.Data;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _http.DeleteAsync($"/api/videos/{id}");
    }
}
