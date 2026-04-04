using LifeTrackerPro.Application.Features.Transactions.Commands;
using LifeTrackerPro.Application.Features.Transactions.Queries;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transactions")
            .RequireAuthorization();

        group.MapGet("/", async (DateOnly? from, DateOnly? to, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTransactionsQuery { From = from, To = to });
            return Results.Ok(result);
        })
        .WithName("GetTransactions");

        group.MapPost("/", async (CreateTransactionRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateTransactionCommand
            {
                Date = request.Date,
                Description = request.Description,
                Amount = request.Amount,
                CategoryTag = request.CategoryTag,
                IsRecurring = request.IsRecurring,
                CurrencyCode = request.CurrencyCode
            });
            return result.Success ? Results.Created($"/api/transactions/{result.Data?.Id}", result) : Results.BadRequest(result);
        })
        .WithName("CreateTransaction");

        group.MapPut("/{id:guid}", async (Guid id, UpdateTransactionRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateTransactionCommand
            {
                Id = id,
                Date = request.Date,
                Description = request.Description,
                Amount = request.Amount,
                CategoryTag = request.CategoryTag,
                IsRecurring = request.IsRecurring,
                CurrencyCode = request.CurrencyCode
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateTransaction");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteTransactionCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("DeleteTransaction");
    }
}
