using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class TransactionService
{
    private readonly HttpClient _http;

    public TransactionService(HttpClient http) => _http = http;

    public async Task<List<TransactionDto>> GetAsync(DateOnly? from = null, DateOnly? to = null)
    {
        var query = "";
        if (from.HasValue) query += $"from={from.Value:yyyy-MM-dd}&";
        if (to.HasValue) query += $"to={to.Value:yyyy-MM-dd}";
        var url = "/api/transactions" + (query.Length > 0 ? "?" + query.TrimEnd('&') : "");
        var result = await _http.GetFromJsonAsync<ApiResponse<List<TransactionDto>>>(url);
        return result?.Data ?? new();
    }

    public async Task<TransactionDto?> CreateAsync(CreateTransactionRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/transactions", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<TransactionDto>>();
        return result?.Data;
    }

    public async Task<TransactionDto?> UpdateAsync(Guid id, UpdateTransactionRequestDto request)
    {
        var response = await _http.PutAsJsonAsync($"/api/transactions/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<TransactionDto>>();
        return result?.Data;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _http.DeleteAsync($"/api/transactions/{id}");
    }
}
