using System.Net.Http.Json;
using LifeTrackerPro.Shared.DTOs;

namespace LifeTrackerPro.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly AuthStateProvider _authState;

    public AuthService(HttpClient http, AuthStateProvider authState)
    {
        _http = http;
        _authState = authState;
    }

    public async Task<ApiResponse<AuthResponseDto>?> RegisterAsync(RegisterRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        if (result?.Success == true && result.Data is not null)
            _authState.SetToken(result.Data.AccessToken, result.Data.User);
        return result;
    }

    public async Task<ApiResponse<AuthResponseDto>?> LoginAsync(LoginRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        if (result?.Success == true && result.Data is not null)
            _authState.SetToken(result.Data.AccessToken, result.Data.User);
        return result;
    }

    public async Task LogoutAsync()
    {
        _authState.ClearToken();
        await Task.CompletedTask;
    }
}
