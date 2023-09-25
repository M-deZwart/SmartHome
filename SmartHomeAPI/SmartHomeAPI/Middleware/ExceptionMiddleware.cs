using SmartHomeAPI.Models;
using System.Net;

namespace SmartHomeAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong {ex.Message}");
            await HandleException(httpContext, ex);
        }
    }

    private ErrorDetails GetErrorDetails(Exception ex)
    {
        var errorDetails = new ErrorDetails()
        {
            Message = ex.Message,
            ClassName = ex.GetType().Name.Split('.').Reverse().First(),
            StackTrace = ex.StackTrace?.Split(Environment.NewLine).ToList()
        };

        if (ex.InnerException is not null)
        {
            errorDetails.InnerException = GetErrorDetails(ex.InnerException);
        }

        return errorDetails;
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = environment == Environments.Development;

        if (isDevelopment && ex is not null)
        {
            await context.Response.WriteAsync(GetErrorDetails(ex).ToString());
        }
        else
        {
            await context.Response.WriteAsync(
                new ErrorDetails()
                {
                    Message = "Internal Server Error",
                    ClassName = "Exception"
                }.ToString()
            );
        }
    }
}
