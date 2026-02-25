using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class DailyLogService
{
    private readonly HttpClient _http;

    public DailyLogService(HttpClient http) => _http = http;

    public async Task<DailyLogDto?> GetAsync(DateOnly date)
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<DailyLogDto>>($"/api/daily-logs/{date:yyyy-MM-dd}");
        return result?.Data;
    }

    public async Task<List<DailyLogDto>> GetRangeAsync(DateOnly from, DateOnly to)
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<List<DailyLogDto>>>($"/api/daily-logs?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        return result?.Data ?? new();
    }

    public async Task<DailyLogDto?> UpsertAsync(DateOnly date, UpsertDailyLogRequestDto request)
    {
        var response = await _http.PutAsJsonAsync($"/api/daily-logs/{date:yyyy-MM-dd}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DailyLogDto>>();
        return result?.Data;
    }
}
