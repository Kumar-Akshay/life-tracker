using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LifeTrackerPro.Application.Features.Dashboard.Queries;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public GetDashboardQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var (from, to) = GetDateRange(request);

        var logs = await _context.DailyLogs
            .AsNoTracking()
            .Where(l => l.UserId == _currentUser.UserId && l.Date >= from && l.Date <= to)
            .OrderBy(l => l.Date)
            .ToListAsync(cancellationToken);

        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == _currentUser.UserId && c.IsActive)
            .ToListAsync(cancellationToken);

        var settings = await _context.UserSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _currentUser.UserId, cancellationToken);

        // Aggregate hours per category
        var categoryHoursMap = new Dictionary<string, int>();
        var dailyBreakdowns = new List<DailyBreakdownDto>();

        foreach (var log in logs)
        {
            var slots = JsonSerializer.Deserialize<List<string>>(log.HourSlots) ?? new List<string>();
            var dayBreakdown = new Dictionary<string, int>();

            foreach (var slot in slots)
            {
                if (string.IsNullOrEmpty(slot)) continue;

                categoryHoursMap[slot] = categoryHoursMap.GetValueOrDefault(slot) + 1;
                dayBreakdown[slot] = dayBreakdown.GetValueOrDefault(slot) + 1;
            }

            dailyBreakdowns.Add(new DailyBreakdownDto
            {
                Date = log.Date,
                CategoryHours = dayBreakdown
            });
        }

        var totalHours = categoryHoursMap.Values.Sum();
        var totalSpent = logs.Sum(l => l.AmountSpent ?? 0);
        var daysLogged = logs.Count;

        var hoursPerCategory = categoryHoursMap.Select(kvp =>
        {
            var cat = categories.FirstOrDefault(c => c.Code == kvp.Key || c.Letter == kvp.Key);
            return new CategoryHoursDto
            {
                CategoryCode = kvp.Key,
                CategoryName = cat?.Name ?? kvp.Key,
                Color = cat?.Color ?? "#6B7280",
                Hours = kvp.Value,
                Percentage = totalHours > 0 ? Math.Round((double)kvp.Value / totalHours * 100, 1) : 0
            };
        }).OrderByDescending(c => c.Hours).ToList();

        // Calculate streak
        var streak = CalculateStreak(logs.Select(l => l.Date).ToList());

        var daysInRange = to.DayNumber - from.DayNumber + 1;

        var dashboard = new DashboardDto
        {
            TotalLoggedHours = totalHours,
            TotalSpent = totalSpent,
            CurrentStreak = streak,
            DaysLogged = daysLogged,
            HoursPerCategory = hoursPerCategory,
            DailyBreakdown = dailyBreakdowns,
            SpendSummary = new SpendSummaryDto
            {
                TotalSpent = totalSpent,
                AverageDaily = daysLogged > 0 ? Math.Round(totalSpent / daysLogged, 2) : 0,
                BudgetRemaining = settings?.MonthlyBudget.HasValue == true
                    ? settings.MonthlyBudget.Value - totalSpent
                    : null,
                CurrencyCode = settings?.CurrencyCode ?? "INR"
            }
        };

        return ApiResponse<DashboardDto>.Ok(dashboard);
    }

    private (DateOnly from, DateOnly to) GetDateRange(GetDashboardQuery request)
    {
        if (request.From.HasValue && request.To.HasValue)
            return (request.From.Value, request.To.Value);

        var today = _dateTime.Today;
        return request.Period.ToLower() switch
        {
            "week" => (today.AddDays(-(int)today.DayOfWeek + 1), today),
            "month" => (new DateOnly(today.Year, today.Month, 1), today),
            "year" => (new DateOnly(today.Year, 1, 1), today),
            _ => (today.AddDays(-7), today)
        };
    }

    private static int CalculateStreak(List<DateOnly> dates)
    {
        if (dates.Count == 0) return 0;

        var sorted = dates.OrderByDescending(d => d).ToList();
        int streak = 1;

        for (int i = 1; i < sorted.Count; i++)
        {
            if (sorted[i - 1].DayNumber - sorted[i].DayNumber == 1)
                streak++;
            else
                break;
        }

        return streak;
    }
}
