using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Enums;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Videos.Queries;

public class GetVideosQueryHandler : IRequestHandler<GetVideosQuery, ApiResponse<List<VideoDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetVideosQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<VideoDto>>> Handle(GetVideosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Videos
            .AsNoTracking()
            .Where(v => v.UserId == _currentUser.UserId);

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<VideoStatus>(request.Status, out var status))
            query = query.Where(v => v.Status == status);

        var videos = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<VideoDto>>.Ok(_mapper.Map<List<VideoDto>>(videos));
    }
}
