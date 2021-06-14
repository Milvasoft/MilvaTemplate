using Fody;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers;
using Milvasoft.Helpers.DependencyInjection;
using Milvasoft.Helpers.Enums;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Models.Response;
using Milvasoft.Helpers.Utils;
using MilvaTemplate.Localization;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Middlewares
{
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
            void SendExceptionMail(Exception ex)
            {
                var logger = context.RequestServices.GetRequiredService<IMilvaLogger>();

                using var sr = new StringReader(ex.StackTrace);

                var path = context.Request.Path;

                var stackTraceFirstLine = sr.ReadLine();

                logger.LogFatal($"{path}|{ex.Message}|{stackTraceFirstLine}", MailSubject.Error);
            }

            var sharedLocalizer = context.RequestServices.GetLocalizerInstance<SharedResource>();

            string message = sharedLocalizer["MiddlewareGeneralErrorMessage"];

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
                    else message = sharedLocalizer[userFriendlyEx.Message, userFriendlyEx.ExceptionObject];

                    errorCodes.Add(userFriendlyEx.ExceptionCode);

                    if (userFriendlyEx.ExceptionCode == (int)MilvaException.CannotGetResponse)
                    {
                        //SendExceptionMail(baseEx);
                    }
                }
                else
                {

                    if (ex is OverflowException || ex is StackOverflowException) message = "Please enter a valid value!";
                    else message = ex.Message;//Prodda burası kapatılacak.
                    Log.Logger.Error(ex, ex.Message);
                    //var logger = context.RequestServices.GetRequiredService<IMilvaLogger>();
                    //logger.LogError(ex.Message);
                    //SendExceptionMail(ex);
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
                context.Response.ContentType = "application/json";
                context.Items.Remove("ActionContent");
                context.Response.StatusCode = MilvaStatusCodes.Status200OK;
                await context.Response.WriteAsync(json);
            }
        }
    }
}
