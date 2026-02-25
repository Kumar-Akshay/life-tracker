using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Application.Features.DailyLogs.Commands;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text.Json;

namespace LifeTrackerPro.UnitTests.Handlers;

public class UpsertDailyLogCommandHandlerTests
{
    private readonly IApplicationDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpsertDailyLogCommandHandler _handler;
    private readonly Guid _userId = Guid.NewGuid();

    public UpsertDailyLogCommandHandlerTests()
    {
        // In-memory EF context
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);

        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);

        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(m => m.Map<DailyLogDto>(It.IsAny<DailyLog>()))
            .Returns((DailyLog src) => new DailyLogDto
            {
                Id = src.Id,
                Date = src.Date,
                HourSlots = JsonSerializer.Deserialize<List<string>>(src.HourSlots ?? "[]") ?? new(),
                Weight = src.Weight,
                AmountSpent = src.AmountSpent,
                Highlight = src.Highlight,
                MoodScore = src.MoodScore,
                ProductivityScore = src.ProductivityScore
            });

        _handler = new UpsertDailyLogCommandHandler(_context, _currentUserMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Should_Create_New_DailyLog_When_None_Exists()
    {
        var cmd = new UpsertDailyLogCommand
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            HourSlots = Enumerable.Repeat("S", 24).ToList(),
            MoodScore = 4,
            Highlight = "Great day"
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(24, result.Data!.HourSlots.Count);
        Assert.Equal("Great day", result.Data.Highlight);

        // Verify persisted
        var saved = await _context.DailyLogs.FirstOrDefaultAsync(d => d.UserId == _userId);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task Should_Update_Existing_DailyLog()
    {
        var date = DateOnly.FromDateTime(DateTime.Today);

        // Pre-seed a log
        _context.DailyLogs.Add(new DailyLog
        {
            UserId = _userId,
            Date = date,
            HourSlots = JsonSerializer.Serialize(Enumerable.Repeat("S", 24)),
            MoodScore = 3
        });
        await _context.SaveChangesAsync();

        // Upsert with different values
        var cmd = new UpsertDailyLogCommand
        {
            Date = date,
            HourSlots = Enumerable.Repeat("W", 24).ToList(),
            MoodScore = 5,
            Highlight = "Updated"
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(5, result.Data!.MoodScore);

        // Only one entry for this date
        var count = await _context.DailyLogs.CountAsync(d => d.UserId == _userId && d.Date == date);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Should_Persist_Weight_And_Amount()
    {
        var cmd = new UpsertDailyLogCommand
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            HourSlots = Enumerable.Repeat("S", 24).ToList(),
            Weight = 72.5m,
            AmountSpent = 150.00m
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Success);
        var saved = await _context.DailyLogs.FirstAsync(d => d.UserId == _userId);
        Assert.Equal(72.5m, saved.Weight);
        Assert.Equal(150.00m, saved.AmountSpent);
    }
}
