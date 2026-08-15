using System.Net;
using System.Text.Json;
using HeritageMarket.Application.Common;

namespace HeritageMarket.Web.Middleware;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleAsync(context, ex);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            InsufficientStockException => HttpStatusCode.BadRequest,
            ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.InternalServerError
        };

        var isApiRequest = context.Request.Path.StartsWithSegments("/api");

        if (isApiRequest || context.Request.Headers.Accept.ToString().Contains("application/json"))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new
            {
                status = (int)statusCode,
                title = statusCode == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : exception.Message,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            return;
        }

        context.Response.Redirect(statusCode switch
        {
            HttpStatusCode.NotFound => "/Home/NotFound",
            _ => "/Home/Error"
        });
    }
}
