using System.Net;
using System.Text.Json;
using E_Commerce.Common;

namespace E_Commerce.Middleware;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(env.IsDevelopment() ? exception.Message : "Internal Server Error");

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}