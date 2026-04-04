using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Books.Queries;

public record GetBooksQuery : IRequest<ApiResponse<List<BookDto>>>
{
    public string? Status { get; init; }   // optional filter
}
