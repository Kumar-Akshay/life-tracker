using LifeTrackerPro.Application.Features.Settings.Commands;
using LifeTrackerPro.Application.Features.Settings.Queries;
using LifeTrackerPro.Shared.Constants;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class SettingsEndpoints
{
    public static void MapSettingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Settings)
            .WithTags("Settings")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSettingsQuery());
            return Results.Ok(result);
        })
        .WithName("GetSettings");

        group.MapPut("/", async (UpdateSettingsRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateSettingsCommand
            {
                TimeGranularity = request.TimeGranularity,
                ThemeMode = request.ThemeMode,
                AiAutoFillEnabled = request.AiAutoFillEnabled,
                MonthlyBudget = request.MonthlyBudget,
                CurrencyCode = request.CurrencyCode,
                Timezone = request.Timezone,
                NotificationsEnabled = request.NotificationsEnabled,
                WeekStartDay = request.WeekStartDay
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateSettings");
    }
}
