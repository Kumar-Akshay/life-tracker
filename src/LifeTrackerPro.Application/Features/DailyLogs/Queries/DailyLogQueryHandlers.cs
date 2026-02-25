using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.DailyLogs.Queries;

public class GetDailyLogQueryHandler : IRequestHandler<GetDailyLogQuery, ApiResponse<DailyLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetDailyLogQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DailyLogDto>> Handle(GetDailyLogQuery request, CancellationToken cancellationToken)
    {
        var log = await _context.DailyLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.UserId == _currentUser.UserId && l.Date == request.Date, cancellationToken);

        if (log is null)
            return ApiResponse<DailyLogDto>.Ok(new DailyLogDto { Date = request.Date });

        return ApiResponse<DailyLogDto>.Ok(_mapper.Map<DailyLogDto>(log));
    }
}

public class GetDailyLogsQueryHandler : IRequestHandler<GetDailyLogsQuery, ApiResponse<List<DailyLogDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetDailyLogsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<DailyLogDto>>> Handle(GetDailyLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _context.DailyLogs
            .AsNoTracking()
            .Where(l => l.UserId == _currentUser.UserId && l.Date >= request.From && l.Date <= request.To)
            .OrderBy(l => l.Date)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<DailyLogDto>>.Ok(_mapper.Map<List<DailyLogDto>>(logs));
    }
}
