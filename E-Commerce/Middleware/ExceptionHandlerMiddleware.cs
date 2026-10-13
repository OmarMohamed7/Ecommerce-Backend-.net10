using System.Net;
using System.Text.Json;
using E_Commerce.DTOs;

namespace E_Commerce.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next, IWebHostEnvironment env,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, exception.Message);
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(_env.IsDevelopment() ? exception.Message : "Internal Server Error");

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}