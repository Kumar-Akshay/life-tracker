using LifeTrackerPro.Application.Features.Books.Commands;
using LifeTrackerPro.Application.Features.Books.Queries;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/books")
            .WithTags("Books")
            .RequireAuthorization();

        group.MapGet("/", async (string? status, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBooksQuery { Status = status });
            return Results.Ok(result);
        })
        .WithName("GetBooks");

        group.MapPost("/", async (CreateBookRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateBookCommand
            {
                Title = request.Title,
                Author = request.Author,
                TotalPages = request.TotalPages,
                Color = request.Color ?? "#6366F1",
                Notes = request.Notes
            });
            return result.Success ? Results.Created($"/api/books/{result.Data?.Id}", result) : Results.BadRequest(result);
        })
        .WithName("CreateBook");

        group.MapPut("/{id:guid}", async (Guid id, UpdateBookRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateBookCommand
            {
                Id = id,
                Title = request.Title,
                Author = request.Author,
                TotalPages = request.TotalPages,
                CurrentPage = request.CurrentPage,
                Status = request.Status,
                Rating = request.Rating,
                StartDate = request.StartDate,
                FinishDate = request.FinishDate,
                Notes = request.Notes,
                Color = request.Color
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateBook");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteBookCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("DeleteBook");
    }
}
