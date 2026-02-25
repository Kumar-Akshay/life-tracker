using AutoMapper;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Application.Features.Categories.Commands;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LifeTrackerPro.UnitTests.Handlers;

public class CategoryCommandHandlerTests
{
    private readonly TestDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Guid _userId = Guid.NewGuid();

    public CategoryCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);

        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(x => x.UserId).Returns(_userId);

        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns((Category src) => new CategoryDto
            {
                Id = src.Id,
                Code = src.Code,
                Name = src.Name,
                Letter = src.Letter,
                Color = src.Color,
                SortOrder = src.SortOrder,
                GroupName = src.GroupName,
                IsArchived = src.IsArchived,
                IsActive = src.IsActive
            });
    }

    [Fact]
    public async Task CreateCategory_Should_Succeed()
    {
        var handler = new CreateCategoryCommandHandler(_context, _currentUserMock.Object, _mapperMock.Object);

        var cmd = new CreateCategoryCommand
        {
            Code = "EX",
            Name = "Exercise",
            Letter = "E",
            Color = "#22C55E",
            GroupName = "Health"
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Exercise", result.Data!.Name);
        Assert.Equal(1, result.Data.SortOrder);

        var saved = await _context.Categories.CountAsync();
        Assert.Equal(1, saved);
    }

    [Fact]
    public async Task CreateCategory_SortOrder_Should_Increment()
    {
        // Seed one category
        _context.Categories.Add(new Category
        {
            UserId = _userId,
            Code = "SL",
            Name = "Sleep",
            Letter = "S",
            SortOrder = 5
        });
        await _context.SaveChangesAsync();

        var handler = new CreateCategoryCommandHandler(_context, _currentUserMock.Object, _mapperMock.Object);

        var cmd = new CreateCategoryCommand
        {
            Code = "WK",
            Name = "Work",
            Letter = "W",
            Color = "#3B82F6"
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.Equal(6, result.Data!.SortOrder);
    }

    [Fact]
    public async Task ArchiveCategory_Should_Set_Flags()
    {
        var cat = new Category
        {
            UserId = _userId,
            Code = "EX",
            Name = "Exercise",
            Letter = "E",
            IsActive = true,
            IsArchived = false
        };
        _context.Categories.Add(cat);
        await _context.SaveChangesAsync();

        var handler = new ArchiveCategoryCommandHandler(_context, _currentUserMock.Object);
        var result = await handler.Handle(new ArchiveCategoryCommand(cat.Id), CancellationToken.None);

        Assert.True(result.Success);

        var saved = await _context.Categories.FindAsync(cat.Id);
        Assert.True(saved!.IsArchived);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task RestoreCategory_Should_Reset_Flags()
    {
        var cat = new Category
        {
            UserId = _userId,
            Code = "EX",
            Name = "Exercise",
            Letter = "E",
            IsActive = false,
            IsArchived = true
        };
        _context.Categories.Add(cat);
        await _context.SaveChangesAsync();

        var handler = new RestoreCategoryCommandHandler(_context, _currentUserMock.Object);
        var result = await handler.Handle(new RestoreCategoryCommand(cat.Id), CancellationToken.None);

        Assert.True(result.Success);

        var saved = await _context.Categories.FindAsync(cat.Id);
        Assert.False(saved!.IsArchived);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task DeleteCategory_Should_Remove_Entity()
    {
        var cat = new Category
        {
            UserId = _userId,
            Code = "X",
            Name = "ToDelete",
            Letter = "X"
        };
        _context.Categories.Add(cat);
        await _context.SaveChangesAsync();

        var handler = new DeleteCategoryCommandHandler(_context, _currentUserMock.Object);
        var result = await handler.Handle(new DeleteCategoryCommand(cat.Id), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(0, await _context.Categories.CountAsync());
    }

    [Fact]
    public async Task ReorderCategories_Should_Update_SortOrders()
    {
        var cat1 = new Category { UserId = _userId, Code = "A", Name = "A", Letter = "A", SortOrder = 1 };
        var cat2 = new Category { UserId = _userId, Code = "B", Name = "B", Letter = "B", SortOrder = 2 };
        _context.Categories.AddRange(cat1, cat2);
        await _context.SaveChangesAsync();

        var handler = new ReorderCategoriesCommandHandler(_context, _currentUserMock.Object);
        var cmd = new ReorderCategoriesCommand
        {
            Items = new List<CategoryOrderItem>
            {
                new() { Id = cat1.Id, SortOrder = 2 },
                new() { Id = cat2.Id, SortOrder = 1 }
            }
        };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result.Success);

        var a = await _context.Categories.FindAsync(cat1.Id);
        var b = await _context.Categories.FindAsync(cat2.Id);
        Assert.Equal(2, a!.SortOrder);
        Assert.Equal(1, b!.SortOrder);
    }
}
