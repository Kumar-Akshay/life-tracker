using LifeTrackerPro.Application.Features.Dashboard.Queries;
using LifeTrackerPro.Shared.Constants;
using MediatR;

namespace LifeTrackerPro.API;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Dashboard)
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/", async (string? period, DateOnly? from, DateOnly? to, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetDashboardQuery
            {
                Period = period ?? "week",
                From = from,
                To = to
            });
            return Results.Ok(result);
        })
        .WithName("GetDashboard");
    }
}
