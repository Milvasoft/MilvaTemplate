using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers.DependencyInjection;
using Milvasoft.Helpers.Mail;
using Milvasoft.Helpers.Models.Response;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using Newtonsoft.Json;
using Serilog;
using System.IO;

namespace MilvaTemplate.API.Middlewares;

/// <summary>
/// Catches errors occurring elsewhere in the project.
/// </summary>
[ConfigureAwait(false)]
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor of <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next"></param>
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the method or constructor reflected by this MethodInfo instance.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var sharedLocalizer = context.RequestServices.GetLocalizerInstance<SharedResource>();

        string message = sharedLocalizer[nameof(ResourceKey.MiddlewareGeneralErrorMessage)];

        List<int> errorCodes = new();

        try
        {
            context.Request.EnableBuffering();

            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            if (ex is MilvaUserFriendlyException userFriendlyEx)
            {
                if (userFriendlyEx.ExceptionObject == null)
                    message = sharedLocalizer[userFriendlyEx.Message];
                else
                {
                    if (!userFriendlyEx.UseLocalizerKey
                        && userFriendlyEx.ExceptionCode == (int)MilvaException.CannotFindEntity
                        && !userFriendlyEx.ExceptionObject.ToList().IsNullOrEmpty())
                    {
                        message = sharedLocalizer[nameof(ResourceKey.CannotFoundMessage), sharedLocalizer[userFriendlyEx.ExceptionObject[0].ToString()]];
                    }
                    else message = sharedLocalizer[userFriendlyEx.Message, userFriendlyEx.ExceptionObject];
                }

                errorCodes.Add(userFriendlyEx.ExceptionCode);

                //If the exception is thrown due to not being able to connect to a service.
                if (userFriendlyEx.ExceptionCode == (int)MilvaException.CannotGetResponse)
                    _ = SendExceptionMail(ex);

                if (!GlobalConstant.RealProduction)
                {
                    var jsonData = await context.Request.ReadBodyFromRequestAsync();

                    var messageTemplate = ex.Message + " ------ {@InnerException} ------ {@zBody} ------ {@RequestPath}";

                    Log.Logger.Error(ex, messageTemplate, ex.InnerException?.Message ?? "void", jsonData ?? "void", context.Request.Path.Value);
                }
            }
            else
            {
                if (GlobalConstant.RealProduction)
                {
                    if (ex is OverflowException or StackOverflowException)
                    {
                        message = sharedLocalizer[nameof(ResourceKey.PleaseEnterAValidValue)];
                    }
                    else if (ex is MilvaDeveloperException devEx)
                    {
                        if (devEx.ExceptionCode == (int)MilvaException.InvalidTenantId)
                            message = sharedLocalizer[nameof(ResourceKey.PleaseEnterAValidValue)];
                    }
                    else
                    {
                        message = sharedLocalizer[nameof(ResourceKey.AnErrorOccured)];

                        var logger = context.RequestServices.GetRequiredService<IMilvaLogger>();

                        logger.Write(SeriLogEventLevel.Fatal, ex, ex.Message);

                        _ = SendExceptionMail(ex);
                    }
                }
                else
                {
                    var jsonData = await context.Request.ReadBodyFromRequestAsync();

                    message = ex.Message;

                    var messageTemplate = ex.Message + " ------ {@InnerException} ------ {@zBody} ------ {@RequestPath}";

                    Log.Logger.Error(ex, messageTemplate, ex.InnerException?.Message ?? "void", jsonData ?? "void", context.Request.Path.Value);
                }
            }

            if (!context.Response.HasStarted)
            {
                var response = new ExceptionResponse
                {
                    Message = message,
                    StatusCode = MilvaStatusCodes.Status600Exception,
                    Success = false,
                    Result = new object(),
                    ErrorCodes = errorCodes
                };

                var json = JsonConvert.SerializeObject(response);

                context.Items.Remove(nameof(MethodContentAttribute.ActionContent));
                context.Response.ContentType = MimeTypeNames.ApplicationJson;
                context.Response.StatusCode = MilvaStatusCodes.Status200OK;

                await context.Response.WriteAsync(json);
            }
        }

        async Task SendExceptionMail(Exception ex)
        {
            var mailSender = context.RequestServices.GetRequiredService<IMilvaMailSender>();

            using var sr = new StringReader(ex.StackTrace);

            var path = context.Request.Path;

            var stackTraceFirstLine = await sr.ReadLineAsync();

            await mailSender.MilvaSendMailAsync("errors@yours.com", "Unhandled Exception From MilvaTemplate", $"{path}|{ex.Message}|{stackTraceFirstLine}");
        }
    }
}
