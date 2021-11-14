using Fody;
using Microsoft.AspNetCore.Http;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Extensions;
using MilvaTemplate.API.Helpers.Constants;
using System;
using System.IO;
using System.Threading.Tasks;

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
                throw new MilvaUserFriendlyException("MissingHeaderException", "PageIndex");

            int itemCount;
            httpContextAccessor.HttpContext.Request.Headers.TryGetValue("itemcount", out var itemCountValue);
            if (!itemCountValue.IsNullOrEmpty())
            {
                itemCount = Convert.ToInt32(itemCountValue[0]);
                if (itemCount <= GlobalConstant.Zero) throw new MilvaUserFriendlyException("InvalidItemRangeException");
            }
            else
                throw new MilvaUserFriendlyException("MissingHeaderException", "ItemCount");

            return (pageIndex, itemCount);
        }
        else throw new MilvaUserFriendlyException("MissingHeaderException", "PageIndex,ItemCount");
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
}
