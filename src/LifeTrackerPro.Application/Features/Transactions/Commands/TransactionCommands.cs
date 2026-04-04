using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Transactions.Commands;

public record CreateTransactionCommand : IRequest<ApiResponse<TransactionDto>>
{
    public DateOnly Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? CategoryTag { get; init; }
    public bool IsRecurring { get; init; }
    public string CurrencyCode { get; init; } = "INR";
}

public record UpdateTransactionCommand : IRequest<ApiResponse<TransactionDto>>
{
    public Guid Id { get; init; }
    public DateOnly Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? CategoryTag { get; init; }
    public bool IsRecurring { get; init; }
    public string CurrencyCode { get; init; } = "INR";
}

public record DeleteTransactionCommand(Guid Id) : IRequest<ApiResponse<bool>>;
