using LifeTrackerPro.Application.Features.DailyLogs.Commands;
using LifeTrackerPro.Application.Features.DailyLogs.Queries;
using LifeTrackerPro.Shared.Constants;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class DailyLogEndpoints
{
    public static void MapDailyLogEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.DailyLogs)
            .WithTags("Daily Logs")
            .RequireAuthorization();

        group.MapGet("/", async (DateOnly? from, DateOnly? to, IMediator mediator) =>
        {
            var fromDate = from ?? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7);
            var toDate = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var result = await mediator.Send(new GetDailyLogsQuery(fromDate, toDate));
            return Results.Ok(result);
        })
        .WithName("GetDailyLogs");

        group.MapGet("/{date}", async (DateOnly date, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDailyLogQuery(date));
            return Results.Ok(result);
        })
        .WithName("GetDailyLog");

        group.MapPut("/{date}", async (DateOnly date, UpsertDailyLogRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpsertDailyLogCommand
            {
                Date = date,
                HourSlots = request.HourSlots,
                Weight = request.Weight,
                AmountSpent = request.AmountSpent,
                Highlight = request.Highlight,
                MoodScore = request.MoodScore,
                ProductivityScore = request.ProductivityScore,
                Notes = request.Notes
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpsertDailyLog");
    }
}
