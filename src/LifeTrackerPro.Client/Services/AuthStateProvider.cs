using LifeTrackerPro.Shared.DTOs;
using System.Net.Http.Headers;

namespace LifeTrackerPro.Client.Services;

public class AuthStateProvider
{
    private readonly HttpClient _http;
    private string? _accessToken;

    public UserProfileDto? CurrentUser { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);

    public event Action? OnAuthStateChanged;

    public AuthStateProvider(HttpClient http)
    {
        _http = http;
    }

    public void SetToken(string accessToken, UserProfileDto user)
    {
        _accessToken = accessToken;
        CurrentUser = user;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        OnAuthStateChanged?.Invoke();
    }

    public void ClearToken()
    {
        _accessToken = null;
        CurrentUser = null;
        _http.DefaultRequestHeaders.Authorization = null;
        OnAuthStateChanged?.Invoke();
    }
}
