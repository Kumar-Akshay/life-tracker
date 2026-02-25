using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using LifeTrackerPro.Client;
using LifeTrackerPro.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to point to API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// MudBlazor
builder.Services.AddMudServices();

// App Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DailyLogService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<AuthStateProvider>();

await builder.Build().RunAsync();
