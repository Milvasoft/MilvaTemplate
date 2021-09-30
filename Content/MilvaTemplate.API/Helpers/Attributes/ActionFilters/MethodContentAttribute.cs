using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using MilvaTemplate.API.Helpers.Extensions;
using MilvaTemplate.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MilvaTemplate.API.Helpers.Attributes.ActionFilters
{
    /// <summary>
    /// Specifies that the class or method that this attribute is applied to requires the specified OPS!TON API authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodContentAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The content in which the method performs the transaction.
        /// </summary>
        public string ActionContent { get; set; }

        /// <summary>
        /// If this value is true, attribute will not add "ActionContent" to Http.Items.
        /// </summary>
        public bool LogMessage { get; set; } = false;

        /// <summary>
        /// Performs when action executed.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!string.IsNullOrEmpty(ActionContent))
            {
                var localizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<SharedResource>>();

                var requestMethod = context.HttpContext.Request.Method;
                if (HttpMethods.IsPost(requestMethod) || HttpMethods.IsPut(requestMethod) || HttpMethods.IsDelete(requestMethod))
                {
                    var defaultLanguageIsoCode = HelperExtensions.LanguageIdIsoPairs.FirstOrDefault(i => i.Value == GlobalConstants.DefaultLanguageId).Key;

                    Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");

                    var actionContent = LogMessage ? localizer[ActionContent]
                                                   : localizer[$"LocalizedEntityName{ActionContent}"] + " " + localizer[$"Action{requestMethod}"];

                    context.HttpContext.Items.Add(new KeyValuePair<object, object>("ActionContent", actionContent));
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
