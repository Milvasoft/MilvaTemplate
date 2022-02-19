using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Net.Http;

namespace MilvaTemplate.API.Helpers.Attributes.ActionFilters;

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
        if (!string.IsNullOrWhiteSpace(ActionContent))
        {
            var sharedLocalizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<SharedResource>>();

            var requestMethod = context.HttpContext.Request.Method;

            if (requestMethod == HttpMethod.Put.ToString() || requestMethod == HttpMethod.Post.ToString() || requestMethod == HttpMethod.Delete.ToString())
            {
                var defaultLanguageIsoCode = HelperExtensions.LanguageIdIsoPairs.FirstOrDefault(i => i.Value == GlobalConstant.DefaultLanguageId).Key;

                if (!string.IsNullOrWhiteSpace(defaultLanguageIsoCode))
                    CultureInfo.CurrentCulture = new CultureInfo(defaultLanguageIsoCode);

                var actionContent = LogMessage ? sharedLocalizer[ActionContent]
                                               : sharedLocalizer[$"{LocalizerKeys.LocalizedEntityName}{ActionContent}"] + " "
                                                    + sharedLocalizer[$"{StringKey.Action}{requestMethod}"];

                context.HttpContext.Items.Add(new KeyValuePair<object, object>(nameof(ActionContent), actionContent));
            }
        }

        base.OnActionExecuted(context);
    }
}
