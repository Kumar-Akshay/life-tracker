using LifeTrackerPro.Application.Features.Categories.Commands;
using LifeTrackerPro.Application.Features.Categories.Queries;
using LifeTrackerPro.Shared.Constants;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Categories)
            .WithTags("Categories")
            .RequireAuthorization();

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery());
            return Results.Ok(result);
        })
        .WithName("GetCategories");

        group.MapPost("/", async (CreateCategoryRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateCategoryCommand
            {
                Code = request.Code,
                Name = request.Name,
                Letter = request.Letter,
                Color = request.Color,
                IconName = request.IconName,
                GroupName = request.GroupName,
                Description = request.Description
            });
            return result.Success ? Results.Created($"/api/categories/{result.Data?.Id}", result) : Results.BadRequest(result);
        })
        .WithName("CreateCategory");

        group.MapPut("/{id:guid}", async (Guid id, UpdateCategoryRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateCategoryCommand
            {
                Id = id,
                Name = request.Name,
                Letter = request.Letter,
                Color = request.Color,
                IconName = request.IconName,
                GroupName = request.GroupName,
                Description = request.Description,
                SortOrder = request.SortOrder
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("UpdateCategory");

        group.MapPut("/{id:guid}/archive", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new ArchiveCategoryCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("ArchiveCategory");

        group.MapPut("/{id:guid}/restore", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new RestoreCategoryCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("RestoreCategory");

        group.MapPut("/reorder", async (ReorderCategoriesRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new ReorderCategoriesCommand { Items = request.Items });
            return Results.Ok(result);
        })
        .WithName("ReorderCategories");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id));
            return result.Success ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("DeleteCategory");
    }
}
