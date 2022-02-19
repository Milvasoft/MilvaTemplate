using Microsoft.AspNetCore.Http;
using System.IO;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
[ConfigureAwait(false)]
public static partial class HelperExtensions
{
    #region HttpContextAccessor Helpers

    /// <summary>
    /// Gets institution id from request's header. Then returns that id variable.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public static (int pageIndex, int itemCount) GetPaginationVariablesFromHeader(this IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("pageindex") && httpContextAccessor.HttpContext.Request.Headers.ContainsKey("itemcount"))
        {
            int pageIndex;
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("pageindex", out var pageIndexValue);
            if (!pageIndexValue.IsNullOrEmpty())
            {
                pageIndex = Convert.ToInt32(pageIndexValue[0]);
                if (pageIndex <= GlobalConstant.Zero) throw new MilvaUserFriendlyException("InvalidPageIndexException");
            }
            else
                throw new MilvaUserFriendlyException(nameof(ResourceKey.MissingHeaderException), "PageIndex");

            int itemCount;
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("itemcount", out var itemCountValue);
            if (!itemCountValue.IsNullOrEmpty())
            {
                itemCount = Convert.ToInt32(itemCountValue[0]);
                if (itemCount <= GlobalConstant.Zero) throw new MilvaUserFriendlyException("InvalidItemRangeException");
            }
            else
                throw new MilvaUserFriendlyException(nameof(ResourceKey.MissingHeaderException), "ItemCount");

            return (pageIndex, itemCount);
        }
        else throw new MilvaUserFriendlyException(nameof(ResourceKey.MissingHeaderException), "PageIndex,ItemCount");
    }

    #endregion  

    /// <summary>
    /// Writes app start information to console.
    /// </summary>
    /// <param name="textWriter"></param>
    /// <param name="message"></param>
    public static void WriteAppInfo(this TextWriter textWriter, string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        textWriter.Write("\n\n info: ");
        Console.ForegroundColor = ConsoleColor.Gray;
        textWriter.Write($"{message}");
    }

    /// <summary>
    /// Writes app start information to console.
    /// </summary>
    /// <param name="textWriter"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static async Task WriteAppInfoAsync(this TextWriter textWriter, string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        await textWriter.WriteAsync("\n\n info: ");
        Console.ForegroundColor = ConsoleColor.Gray;
        await textWriter.WriteAsync($"{message}");
    }

    /// <summary>
    /// Creates projection expression for contents service. THIS IS A AMAZING METHOD.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="propertyNames"></param>
    /// <param name="langProps"></param>
    /// <param name="langType"></param>
    /// <returns></returns>
    public static Expression<Func<TEntity, TEntity>> CreateProjectionExpression<TEntity>(this IEnumerable<string> propertyNames, IEnumerable<string> langProps, Type langType = null)
    {
        var sourceAndResultType = typeof(TEntity);

        LambdaExpression langExpression = null;

        if (!langProps.IsNullOrEmpty())
        {
            langProps = langProps.Append("SystemLanguageId");

            var langParameter = Expression.Parameter(langType, "l");

            var langBindings = langProps.Select(column => Expression.Bind(langType.GetProperty(column), Expression.Property(langParameter, column)));

            var langBody = Expression.MemberInit(Expression.New(langType), langBindings);

            langExpression = Expression.Lambda(langBody, langParameter);
        }

        var parameter = Expression.Parameter(sourceAndResultType, "c");

        MethodCallExpression selectExpressionForLangs = null;

        if (langExpression != null)
        {
            selectExpressionForLangs = Expression.Call(typeof(Enumerable),
                                                       nameof(Enumerable.Select),
                                                       new Type[] { langType, langType },
                                                       Expression.PropertyOrField(parameter, $"{langType.Name}s"),
                                                       langExpression);
        }



        var bindings = propertyNames.Select(column => Expression.Bind(sourceAndResultType.GetProperty(column), column.EndsWith("Langs") ? selectExpressionForLangs : Expression.Property(parameter, column)));

        var body = Expression.MemberInit(Expression.New(sourceAndResultType), bindings);

        return Expression.Lambda<Func<TEntity, TEntity>>(body, parameter);
    }

    /// <summary>
    /// Reads body from http request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<string> ReadBodyFromRequestAsync(this HttpRequest request)
    {
        string json = null;

        if (request.HasJsonContentType())
        {
            if (request.Body.CanRead)
            {
                Stream requestStream = request.Body;

                requestStream.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(requestStream);

                json = await reader.ReadToEndAsync();

                request.Body.Position = 0;
            }
        }

        return json;
    }
}
