using FluentValidation;
using LifeTrackerPro.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace LifeTrackerPro.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                (int)HttpStatusCode.BadRequest,
                "Validation failed.",
                validationEx.Errors.Select(e => e.ErrorMessage).ToList()),

            NotFoundException => (
                (int)HttpStatusCode.NotFound,
                exception.Message,
                new List<string>()),

            ForbiddenAccessException => (
                (int)HttpStatusCode.Forbidden,
                exception.Message,
                new List<string>()),

            BadRequestException badReqEx => (
                (int)HttpStatusCode.BadRequest,
                badReqEx.Message,
                badReqEx.Errors),

            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized.",
                new List<string>()),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                new List<string>())
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            success = false,
            message,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
