using Microsoft.AspNetCore.Http;
using Milvasoft.Core.Utils.Models.Response;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using MilvaTemplate.Data.Abstract;

namespace MilvaTemplate.API.Middlewares;

/// <summary>
/// Checks if the title is suitable for the conditions.
/// </summary>
[ConfigureAwait(false)]
public class ActivityLoggerMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes new instance of <see cref="ActivityLoggerMiddleware"/>.
    /// </summary>
    /// <param name="next"></param>
    public ActivityLoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the method or constructor reflected by this MethodInfo instance.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext httpContext)
    {
        await _next.Invoke(httpContext);

        if (httpContext.Response.HasStarted)
        {
            var statusCode = httpContext.Items[nameof(BaseResponse.StatusCode)] != null ? (int)httpContext.Items[nameof(BaseResponse.StatusCode)] : 200;

            var actionContent = httpContext.Items[nameof(MethodContentAttribute.ActionContent)];

            if (actionContent != null && statusCode >= 200 && statusCode <= 299)
            {
                var requestMethod = httpContext.Request.Method;

                if (HttpMethods.IsPost(requestMethod) || HttpMethods.IsPut(requestMethod) || HttpMethods.IsDelete(requestMethod))
                {
                    var username = httpContext.User?.Identity?.Name ?? string.Empty;

                    var userActivityLogRepository = (IMilvaTemplateRepositoryBase<UserActivityLog, Guid>)httpContext.RequestServices.GetService(typeof(IMilvaTemplateRepositoryBase<UserActivityLog, Guid>));

                    await userActivityLogRepository.AddAsync(new UserActivityLog
                    {
                        UserName = username,
                        Activity = $"{actionContent}"
                    });
                }
            }
        }
    }
}
