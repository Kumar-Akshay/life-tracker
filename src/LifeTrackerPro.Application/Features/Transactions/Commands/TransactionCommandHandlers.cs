using AutoMapper;
using LifeTrackerPro.Application.Common.Exceptions;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Transactions.Commands;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ApiResponse<TransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = new Transaction
        {
            UserId = _currentUser.UserId,
            Date = request.Date,
            Description = request.Description,
            Amount = request.Amount,
            CategoryTag = request.CategoryTag,
            IsRecurring = request.IsRecurring,
            CurrencyCode = request.CurrencyCode
        };

        _context.Transactions.Add(tx);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<TransactionDto>.Ok(_mapper.Map<TransactionDto>(tx));
    }
}

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, ApiResponse<TransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateTransactionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionDto>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Transaction), request.Id);

        tx.Date = request.Date;
        tx.Description = request.Description;
        tx.Amount = request.Amount;
        tx.CategoryTag = request.CategoryTag;
        tx.IsRecurring = request.IsRecurring;
        tx.CurrencyCode = request.CurrencyCode;
        tx.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<TransactionDto>.Ok(_mapper.Map<TransactionDto>(tx));
    }
}

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteTransactionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Transaction), request.Id);

        _context.Transactions.Remove(tx);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
