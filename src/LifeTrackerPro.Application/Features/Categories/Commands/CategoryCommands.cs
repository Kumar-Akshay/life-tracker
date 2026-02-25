using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Categories.Commands;

public record CreateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Letter { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366F1";
    public string? IconName { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
}

public record UpdateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Letter { get; init; } = string.Empty;
    public string Color { get; init; } = "#6366F1";
    public string? IconName { get; init; }
    public string? GroupName { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
}

public record ArchiveCategoryCommand(Guid Id) : IRequest<ApiResponse<bool>>;
public record RestoreCategoryCommand(Guid Id) : IRequest<ApiResponse<bool>>;
public record DeleteCategoryCommand(Guid Id) : IRequest<ApiResponse<bool>>;

public record ReorderCategoriesCommand : IRequest<ApiResponse<bool>>
{
    public List<CategoryOrderItem> Items { get; init; } = new();
}
