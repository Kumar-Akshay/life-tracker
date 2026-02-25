using AutoMapper;
using LifeTrackerPro.Application.Common.Exceptions;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeTrackerPro.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var maxOrder = await _context.Categories
            .Where(c => c.UserId == _currentUser.UserId)
            .MaxAsync(c => (int?)c.SortOrder, cancellationToken) ?? 0;

        var category = new Category
        {
            UserId = _currentUser.UserId,
            Code = request.Code,
            Name = request.Name,
            Letter = request.Letter,
            Color = request.Color,
            IconName = request.IconName,
            GroupName = request.GroupName,
            Description = request.Description,
            SortOrder = maxOrder + 1,
            IsActive = true
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Name;
        category.Letter = request.Letter;
        category.Color = request.Color;
        category.IconName = request.IconName;
        category.GroupName = request.GroupName;
        category.Description = request.Description;
        category.SortOrder = request.SortOrder;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }
}

public class ArchiveCategoryCommandHandler : IRequestHandler<ArchiveCategoryCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ArchiveCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(ArchiveCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.IsArchived = true;
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}

public class RestoreCategoryCommandHandler : IRequestHandler<RestoreCategoryCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RestoreCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.IsArchived = false;
        category.IsActive = true;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteCategoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}

public class ReorderCategoriesCommandHandler : IRequestHandler<ReorderCategoriesCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ReorderCategoriesCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == _currentUser.UserId)
            .ToListAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            var category = categories.FirstOrDefault(c => c.Id == item.Id);
            if (category is not null)
                category.SortOrder = item.SortOrder;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Ok(true);
    }
}
