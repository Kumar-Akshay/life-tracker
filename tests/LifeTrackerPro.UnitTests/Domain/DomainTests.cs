using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.Constants;

namespace LifeTrackerPro.UnitTests.Domain;

public class DefaultCategoriesTests
{
    [Fact]
    public void DefaultCategories_Should_Have_12_Items()
    {
        Assert.Equal(12, DefaultCategories.All.Count);
    }

    [Fact]
    public void DefaultCategories_Should_Have_Unique_Codes()
    {
        var codes = DefaultCategories.All.Select(c => c.Code).ToList();
        Assert.Equal(codes.Distinct().Count(), codes.Count);
    }

    [Fact]
    public void DefaultCategories_Should_Have_Unique_Letters()
    {
        var letters = DefaultCategories.All.Select(c => c.Letter).ToList();
        Assert.Equal(letters.Distinct().Count(), letters.Count);
    }

    [Fact]
    public void DefaultCategories_Should_All_Have_Colors()
    {
        foreach (var cat in DefaultCategories.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(cat.Color), $"Category {cat.Code} has no color.");
            Assert.StartsWith("#", cat.Color);
        }
    }

    [Fact]
    public void DefaultCategories_Should_All_Have_GroupName()
    {
        foreach (var cat in DefaultCategories.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(cat.GroupName), $"Category {cat.Code} has no group name.");
        }
    }
}

public class DailyLogEntityTests
{
    [Fact]
    public void DailyLog_Should_Default_HourSlots_To_EmptyArray()
    {
        var log = new DailyLog();
        Assert.Equal("[]", log.HourSlots);
    }

    [Fact]
    public void DailyLog_Should_Have_Nullable_Weight()
    {
        var log = new DailyLog();
        Assert.Null(log.Weight);

        log.Weight = 75.5m;
        Assert.Equal(75.5m, log.Weight);
    }
}

public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_Ok_Should_Set_Succeeded()
    {
        var response = LifeTrackerPro.Shared.DTOs.ApiResponse<string>.Ok("test");
        Assert.True(response.Success);
        Assert.Equal("test", response.Data);
    }

    [Fact]
    public void ApiResponse_Fail_Should_Contain_Error()
    {
        var response = LifeTrackerPro.Shared.DTOs.ApiResponse<string>.Fail("error");
        Assert.False(response.Success);
        Assert.Contains("error", response.Errors);
    }
}
