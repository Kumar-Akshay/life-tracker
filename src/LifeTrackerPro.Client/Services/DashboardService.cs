using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class DashboardService
{
    private readonly HttpClient _http;

    public DashboardService(HttpClient http) => _http = http;

    public async Task<DashboardDto?> GetAsync(string period = "week", DateOnly? from = null, DateOnly? to = null)
    {
        var url = $"/api/dashboard?period={period}";
        if (from.HasValue) url += $"&from={from.Value:yyyy-MM-dd}";
        if (to.HasValue) url += $"&to={to.Value:yyyy-MM-dd}";

        var result = await _http.GetFromJsonAsync<ApiResponse<DashboardDto>>(url);
        return result?.Data;
    }
}
