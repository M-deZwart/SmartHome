using System.Runtime.CompilerServices;

namespace Smarthome.Presentation.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
