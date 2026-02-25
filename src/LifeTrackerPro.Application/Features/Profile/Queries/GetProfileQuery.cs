using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Profile.Queries;

public record GetProfileQuery : IRequest<ApiResponse<UserProfileDto>>;
