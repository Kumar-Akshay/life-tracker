using AutoMapper;
using LifeTrackerPro.Application.Common.Exceptions;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Domain.Enums;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Books.Commands;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, ApiResponse<BookDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            UserId = _currentUser.UserId,
            Title = request.Title,
            Author = request.Author,
            TotalPages = request.TotalPages,
            Color = request.Color,
            Notes = request.Notes,
            Status = BookStatus.ToStart
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book));
    }
}

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, ApiResponse<BookDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BookDto>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.Id);

        book.Title = request.Title;
        book.Author = request.Author;
        book.TotalPages = request.TotalPages;
        book.CurrentPage = request.CurrentPage;
        book.Status = Enum.TryParse<BookStatus>(request.Status, out var status) ? status : BookStatus.ToStart;
        book.Rating = request.Rating;
        book.StartDate = request.StartDate;
        book.FinishDate = request.FinishDate;
        book.Notes = request.Notes;
        book.Color = request.Color;
        book.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookDto>.Ok(_mapper.Map<BookDto>(book));
    }
}

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteBookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.Id);

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
