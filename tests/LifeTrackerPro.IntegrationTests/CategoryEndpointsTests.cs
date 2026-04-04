using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.IntegrationTests;

public class CategoryEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CategoryEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var email = $"cats_{Guid.NewGuid():N}@example.com";

        var regResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDto
        {
            Email = email,
            Password = "Test1234!",
            FullName = "Category Test User"
        });

        var regResult = await regResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", regResult!.Data!.AccessToken);

        return client;
    }

    [Fact]
    public async Task GetCategories_AfterRegistration_ReturnsDefaultCategories()
    {
        var client = await GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDto>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        var client = await GetAuthenticatedClientAsync();

        var request = new CreateCategoryRequestDto
        {
            Code = $"TST_{Guid.NewGuid():N[..6]}",
            Name = "Test Category",
            Letter = "T",
            Color = "#FF5733"
        };

        var response = await client.PostAsJsonAsync("/api/categories", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Test Category");
        result.Data.Color.Should().Be("#FF5733");
    }

    [Fact]
    public async Task UpdateCategory_ExistingCategory_ReturnsOk()
    {
        var client = await GetAuthenticatedClientAsync();

        // Create first
        var createResp = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequestDto
        {
            Code = $"UPD_{Guid.NewGuid():N[..6]}",
            Name = "To Update",
            Letter = "U",
            Color = "#AAAAAA"
        });
        var created = await createResp.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        var id = created!.Data!.Id;

        // Then update
        var updateResp = await client.PutAsJsonAsync($"/api/categories/{id}", new UpdateCategoryRequestDto
        {
            Name = "Updated Name",
            Letter = "U",
            Color = "#BBBBBB",
            SortOrder = 99
        });

        updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await updateResp.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        result!.Data!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteCategory_ExistingCategory_ReturnsOk()
    {
        var client = await GetAuthenticatedClientAsync();

        var createResp = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequestDto
        {
            Code = $"DEL_{Guid.NewGuid():N[..6]}",
            Name = "To Delete",
            Letter = "D",
            Color = "#123456"
        });
        var created = await createResp.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        var id = created!.Data!.Id;

        var deleteResp = await client.DeleteAsync($"/api/categories/{id}");

        deleteResp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ArchiveAndRestoreCategory_WorksCorrectly()
    {
        var client = await GetAuthenticatedClientAsync();

        var createResp = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequestDto
        {
            Code = $"ARC_{Guid.NewGuid():N[..6]}",
            Name = "To Archive",
            Letter = "A",
            Color = "#654321"
        });
        var created = await createResp.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
        var id = created!.Data!.Id;

        // Archive
        var archiveResp = await client.PutAsync($"/api/categories/{id}/archive", null);
        archiveResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Restore
        var restoreResp = await client.PutAsync($"/api/categories/{id}/restore", null);
        restoreResp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
