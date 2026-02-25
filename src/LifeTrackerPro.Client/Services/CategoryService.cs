using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class CategoryService
{
    private readonly HttpClient _http;

    public CategoryService(HttpClient http) => _http = http;

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var result = await _http.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("/api/categories");
        return result?.Data ?? new();
    }

    public async Task<CategoryDto?> CreateAsync(CreateCategoryRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/categories", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        return result?.Data;
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequestDto request)
    {
        var response = await _http.PutAsJsonAsync($"/api/categories/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        return result?.Data;
    }

    public async Task ArchiveAsync(Guid id) =>
        await _http.PutAsync($"/api/categories/{id}/archive", null);

    public async Task RestoreAsync(Guid id) =>
        await _http.PutAsync($"/api/categories/{id}/restore", null);

    public async Task DeleteAsync(Guid id) =>
        await _http.DeleteAsync($"/api/categories/{id}");

    public async Task ReorderAsync(ReorderCategoriesRequestDto request) =>
        await _http.PutAsJsonAsync("/api/categories/reorder", request);
}
