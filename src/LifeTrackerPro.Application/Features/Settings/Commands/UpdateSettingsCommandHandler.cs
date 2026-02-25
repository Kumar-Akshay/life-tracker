using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Domain.Enums;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Settings.Commands;

public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, ApiResponse<UserSettingsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateSettingsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserSettingsDto>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await _context.UserSettings
            .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);

        if (settings is null)
        {
            settings = new UserSettings { UserId = _currentUser.UserId };
            _context.UserSettings.Add(settings);
        }

        if (request.TimeGranularity is not null && Enum.TryParse<TimeGranularity>(request.TimeGranularity, true, out var tg))
            settings.TimeGranularity = tg;

        if (request.ThemeMode is not null && Enum.TryParse<ThemeMode>(request.ThemeMode, true, out var tm))
            settings.ThemeMode = tm;

        if (request.AiAutoFillEnabled.HasValue)
            settings.AiAutoFillEnabled = request.AiAutoFillEnabled.Value;

        if (request.MonthlyBudget.HasValue)
            settings.MonthlyBudget = request.MonthlyBudget.Value;

        if (request.CurrencyCode is not null)
            settings.CurrencyCode = request.CurrencyCode;

        if (request.Timezone is not null)
            settings.Timezone = request.Timezone;

        if (request.NotificationsEnabled.HasValue)
            settings.NotificationsEnabled = request.NotificationsEnabled.Value;

        if (request.WeekStartDay is not null)
            settings.WeekStartDay = request.WeekStartDay;

        settings.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<UserSettingsDto>.Ok(_mapper.Map<UserSettingsDto>(settings));
    }
}
