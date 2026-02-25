using LifeTrackerPro.Application.Features.Auth.Commands;
using LifeTrackerPro.Shared.Constants;
using LifeTrackerPro.Shared.DTOs;
using MediatR;

namespace LifeTrackerPro.API;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Auth).WithTags("Auth");

        group.MapPost("/register", async (RegisterRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new RegisterCommand
            {
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName,
                Timezone = request.Timezone
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("Register")
        .AllowAnonymous();

        group.MapPost("/login", async (LoginRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            });
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("Login")
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            });
            return result.Success ? Results.Ok(result) : Results.Unauthorized();
        })
        .WithName("RefreshToken")
        .AllowAnonymous();

        group.MapPost("/logout", async (RefreshTokenRequestDto request, IMediator mediator) =>
        {
            var result = await mediator.Send(new LogoutCommand
            {
                RefreshToken = request.RefreshToken
            });
            return Results.Ok(result);
        })
        .WithName("Logout")
        .RequireAuthorization();
    }
}
