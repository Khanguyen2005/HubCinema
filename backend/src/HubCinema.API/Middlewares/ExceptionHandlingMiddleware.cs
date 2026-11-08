using HubCinema.Common.Exceptions;
using HubCinema.Common.Responses;

namespace HubCinema.API.Middlewares;

/// <summary>
/// Catches exceptions from the pipeline and converts them into a uniform
/// <see cref="ApiResponse"/> with the right HTTP status: known <see cref="AppException"/>s
/// map to their status code; anything else becomes a 500 (details logged, not leaked).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled application exception: {Message}", ex.Message);
            var errors = (ex as BadRequestException)?.Errors;
            await WriteAsync(context, ex.StatusCode, ApiResponse.Fail(ex.Message, errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, StatusCodes.Status500InternalServerError,
                ApiResponse.Fail("An unexpected error occurred."));
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, ApiResponse body)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(body);
    }
}
