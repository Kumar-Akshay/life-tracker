using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LifeTrackerPro.Application.Features.DailyLogs.Commands;

public class UpsertDailyLogCommandHandler : IRequestHandler<UpsertDailyLogCommand, ApiResponse<DailyLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpsertDailyLogCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DailyLogDto>> Handle(UpsertDailyLogCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.DailyLogs
            .FirstOrDefaultAsync(l => l.UserId == _currentUser.UserId && l.Date == request.Date, cancellationToken);

        var hourSlotsJson = JsonSerializer.Serialize(request.HourSlots);

        if (existing is not null)
        {
            existing.HourSlots = hourSlotsJson;
            existing.Weight = request.Weight;
            existing.AmountSpent = request.AmountSpent;
            existing.Highlight = request.Highlight;
            existing.MoodScore = request.MoodScore;
            existing.ProductivityScore = request.ProductivityScore;
            existing.Notes = request.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existing = new DailyLog
            {
                UserId = _currentUser.UserId,
                Date = request.Date,
                HourSlots = hourSlotsJson,
                Weight = request.Weight,
                AmountSpent = request.AmountSpent,
                Highlight = request.Highlight,
                MoodScore = request.MoodScore,
                ProductivityScore = request.ProductivityScore,
                Notes = request.Notes
            };
            _context.DailyLogs.Add(existing);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<DailyLogDto>.Ok(_mapper.Map<DailyLogDto>(existing));
    }
}
