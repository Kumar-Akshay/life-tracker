using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Settings.Queries;

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ApiResponse<UserSettingsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetSettingsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserSettingsDto>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);

        if (settings is null)
            return ApiResponse<UserSettingsDto>.Ok(new UserSettingsDto());

        return ApiResponse<UserSettingsDto>.Ok(_mapper.Map<UserSettingsDto>(settings));
    }
}
