using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Books.Commands;

public record CreateBookCommand : IRequest<ApiResponse<BookDto>>
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int TotalPages { get; init; }
    public string Color { get; init; } = "#6366F1";
    public string? Notes { get; init; }
}

public record UpdateBookCommand : IRequest<ApiResponse<BookDto>>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public string Status { get; init; } = "ToStart";
    public int Rating { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? FinishDate { get; init; }
    public string? Notes { get; init; }
    public string Color { get; init; } = "#6366F1";
}

public record DeleteBookCommand(Guid Id) : IRequest<ApiResponse<bool>>;
