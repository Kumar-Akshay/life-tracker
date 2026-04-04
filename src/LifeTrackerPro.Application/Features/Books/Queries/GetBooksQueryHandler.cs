using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Enums;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Books.Queries;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, ApiResponse<List<BookDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetBooksQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<BookDto>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Books
            .AsNoTracking()
            .Where(b => b.UserId == _currentUser.UserId);

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<BookStatus>(request.Status, out var status))
            query = query.Where(b => b.Status == status);

        var books = await query
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<BookDto>>.Ok(_mapper.Map<List<BookDto>>(books));
    }
}
