using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>;
