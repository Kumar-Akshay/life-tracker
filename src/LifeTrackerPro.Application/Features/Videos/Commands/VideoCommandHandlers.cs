using AutoMapper;
using LifeTrackerPro.Application.Common.Exceptions;
using LifeTrackerPro.Application.Common.Interfaces;
using LifeTrackerPro.Domain.Entities;
using LifeTrackerPro.Domain.Enums;
using LifeTrackerPro.Shared.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LifeTrackerPro.Application.Features.Videos.Commands;

public class CreateVideoCommandHandler : IRequestHandler<CreateVideoCommand, ApiResponse<VideoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CreateVideoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VideoDto>> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        var video = new Video
        {
            UserId = _currentUser.UserId,
            Title = request.Title,
            Platform = request.Platform,
            DurationMinutes = request.DurationMinutes,
            Tags = JsonSerializer.Serialize(request.Tags),
            Notes = request.Notes,
            Status = VideoStatus.ToStart
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<VideoDto>.Ok(_mapper.Map<VideoDto>(video));
    }
}

public class UpdateVideoCommandHandler : IRequestHandler<UpdateVideoCommand, ApiResponse<VideoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateVideoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VideoDto>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Video), request.Id);

        video.Title = request.Title;
        video.Platform = request.Platform;
        video.DurationMinutes = request.DurationMinutes;
        video.ProgressPercent = Math.Clamp(request.ProgressPercent, 0, 100);
        video.Status = Enum.TryParse<VideoStatus>(request.Status, out var status) ? status : VideoStatus.ToStart;
        video.Tags = JsonSerializer.Serialize(request.Tags);
        video.Notes = request.Notes;
        video.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<VideoDto>.Ok(_mapper.Map<VideoDto>(video));
    }
}

public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteVideoCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
    {
        var video = await _context.Videos
            .FirstOrDefaultAsync(v => v.Id == request.Id && v.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Video), request.Id);

        _context.Videos.Remove(video);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.Ok(true);
    }
}
