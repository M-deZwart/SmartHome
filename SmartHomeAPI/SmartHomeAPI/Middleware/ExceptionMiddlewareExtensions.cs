using System.Runtime.CompilerServices;

namespace SmartHomeAPI.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
