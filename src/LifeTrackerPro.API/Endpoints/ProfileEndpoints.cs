using LifeTrackerPro.Application.Features.Profile.Commands;
using LifeTrackerPro.Application.Features.Profile.Queries;
using LifeTrackerPro.Shared.Constants;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Profile)
            .WithTags("Profile")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProfileQuery());
            return Results.Ok(result);
        })
        .WithName("GetProfile");

        group.MapPut("/", async (UpdateProfileRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateProfileCommand
            {
                FullName = request.FullName,
                Timezone = request.Timezone,
                AvatarUrl = request.AvatarUrl
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateProfile");
    }
}
