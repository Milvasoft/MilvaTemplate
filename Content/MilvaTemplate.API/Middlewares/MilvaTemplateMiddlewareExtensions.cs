using Microsoft.AspNetCore.Builder;

namespace MilvaTemplate.API.Middlewares;

/// <summary>
/// With the extension method, we ensure that our custom method is added under IApplicationBuilder.
/// </summary>
public static class ErrorMiddlewareExtension
{
    /// <summary>
    /// Extension method of <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseMilvaTemplateExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }

    /// <summary>
    /// Extension method of <see cref="ActivityLoggerMiddleware"/> class.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseActivityLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ActivityLoggerMiddleware>();
    }
}
