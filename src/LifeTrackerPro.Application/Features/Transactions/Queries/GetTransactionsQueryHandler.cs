using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Transactions.Queries;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, ApiResponse<List<TransactionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IDateTimeService _dateTime;

    public GetTransactionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<ApiResponse<List<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var today = _dateTime.Today;
        var from = request.From ?? new DateOnly(today.Year, today.Month, 1);
        var to = request.To ?? today;

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == _currentUser.UserId && t.Date >= from && t.Date <= to)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<TransactionDto>>.Ok(_mapper.Map<List<TransactionDto>>(transactions));
    }
}
