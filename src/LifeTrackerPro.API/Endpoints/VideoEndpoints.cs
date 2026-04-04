using LifeTrackerPro.Application.Features.Videos.Commands;
using LifeTrackerPro.Application.Features.Videos.Queries;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class VideoEndpoints
{
    public static void MapVideoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/videos")
            .WithTags("Videos")
            .RequireAuthorization();

        group.MapGet("/", async (string? status, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetVideosQuery { Status = status });
            return Results.Ok(result);
        })
        .WithName("GetVideos");

        group.MapPost("/", async (CreateVideoRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateVideoCommand
            {
                Title = request.Title,
                Platform = request.Platform,
                DurationMinutes = request.DurationMinutes,
                Tags = request.Tags,
                Notes = request.Notes
            });
            return result.Success ? Results.Created($"/api/videos/{result.Data?.Id}", result) : Results.BadRequest(result);
        })
        .WithName("CreateVideo");

        group.MapPut("/{id:guid}", async (Guid id, UpdateVideoRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateVideoCommand
            {
                Id = id,
                Title = request.Title,
                Platform = request.Platform,
                DurationMinutes = request.DurationMinutes,
                ProgressPercent = request.ProgressPercent,
                Status = request.Status,
                Tags = request.Tags,
                Notes = request.Notes
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateVideo");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteVideoCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("DeleteVideo");
    }
}
