using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.Application.Features.Settings.Queries;

public record GetSettingsQuery : IRequest<ApiResponse<UserSettingsDto>>;
