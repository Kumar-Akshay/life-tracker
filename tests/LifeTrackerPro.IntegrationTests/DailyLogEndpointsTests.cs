using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.IntegrationTests;

public class DailyLogEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DailyLogEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var email = $"logs_{Guid.NewGuid():N}@example.com";

        var regResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDto
        {
            Email = email,
            Password = "Test1234!",
            FullName = "Log Test User"
        });

        var regResult = await regResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", regResult!.Data!.AccessToken);

        return client;
    }

    [Fact]
    public async Task UpsertDailyLog_WithValidData_ReturnsOk()
    {
        var client = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.Today);

        var request = new UpsertDailyLogRequestDto
        {
            HourSlots = Enumerable.Repeat("S", 8).Concat(Enumerable.Repeat("W", 8)).Concat(Enumerable.Repeat("", 8)).ToList(),
            MoodScore = 4,
            ProductivityScore = 7,
            Highlight = "Great day of testing!"
        };

        var response = await client.PutAsJsonAsync($"/api/daily-logs/{date:yyyy-MM-dd}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DailyLogDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Date.Should().Be(date);
        result.Data.MoodScore.Should().Be(4);
        result.Data.ProductivityScore.Should().Be(7);
    }

    [Fact]
    public async Task GetDailyLog_ForExistingDate_ReturnsLog()
    {
        var client = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Create a log first
        await client.PutAsJsonAsync($"/api/daily-logs/{date:yyyy-MM-dd}", new UpsertDailyLogRequestDto
        {
            HourSlots = Enumerable.Repeat("S", 24).ToList(),
            Highlight = "Sleep day"
        });

        // Fetch it back
        var response = await client.GetAsync($"/api/daily-logs/{date:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DailyLogDto>>();
        result!.Data!.Highlight.Should().Be("Sleep day");
    }

    [Fact]
    public async Task GetDailyLogs_Range_ReturnsMultipleLogs()
    {
        var client = await GetAuthenticatedClientAsync();
        var today = DateOnly.FromDateTime(DateTime.Today);

        // Create two logs
        for (int i = 0; i < 2; i++)
        {
            var d = today.AddDays(-i);
            await client.PutAsJsonAsync($"/api/daily-logs/{d:yyyy-MM-dd}", new UpsertDailyLogRequestDto
            {
                HourSlots = Enumerable.Repeat("W", 24).ToList()
            });
        }

        var from = today.AddDays(-1);
        var response = await client.GetAsync($"/api/daily-logs?from={from:yyyy-MM-dd}&to={today:yyyy-MM-dd}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DailyLogDto>>>();
        result!.Data!.Count.Should().Be(2);
    }

    [Fact]
    public async Task UpsertDailyLog_TwiceOnSameDate_UpdatesExisting()
    {
        var client = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.Today.AddDays(-2));

        await client.PutAsJsonAsync($"/api/daily-logs/{date:yyyy-MM-dd}", new UpsertDailyLogRequestDto
        {
            HourSlots = Enumerable.Repeat("S", 24).ToList(),
            Highlight = "First write"
        });

        var response = await client.PutAsJsonAsync($"/api/daily-logs/{date:yyyy-MM-dd}", new UpsertDailyLogRequestDto
        {
            HourSlots = Enumerable.Repeat("W", 24).ToList(),
            Highlight = "Second write"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DailyLogDto>>();
        result!.Data!.Highlight.Should().Be("Second write");
    }
}
