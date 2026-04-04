using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Transactions.Queries;

public record GetTransactionsQuery : IRequest<ApiResponse<List<TransactionDto>>>
{
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
}
