using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class BookService
{
    private readonly HttpClient _http;

    public BookService(HttpClient http) => _http = http;

    public async Task<List<BookDto>> GetAllAsync(string? status = null)
    {
        var url = "/api/books" + (status is not null ? $"?status={status}" : "");
        var result = await _http.GetFromJsonAsync<ApiResponse<List<BookDto>>>(url);
        return result?.Data ?? new();
    }

    public async Task<BookDto?> CreateAsync(CreateBookRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/books", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
        return result?.Data;
    }

    public async Task<BookDto?> UpdateAsync(Guid id, UpdateBookRequestDto request)
    {
        var response = await _http.PutAsJsonAsync($"/api/books/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();
        return result?.Data;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _http.DeleteAsync($"/api/books/{id}");
    }
}
