using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilvaTemplate.API.DTOs.ContentDTOs;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using MilvaTemplate.API.Services.Abstract;

namespace MilvaTemplate.API.Controllers;

/// <summary>
/// Provides get contents which independent anywhere.
/// </summary>
/// <returns></returns>
[Route(GlobalConstant.FullRoute)]
[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1.0")]
[Authorize(Roles = RoleName.Administrator)]
[ConfigureAwait(false)]
public class ContentsController : ControllerBase
{
    private readonly IContentService _contentService;

    /// <summary>
    /// Constructor of <c>ContentsController</c>
    /// </summary>
    /// <param name="contentService"></param>
    public ContentsController(IContentService contentService)
    {
        _contentService = contentService;
    }

    /// <summary>
    /// Gets required content by contentParameters.EntityName.
    /// </summary>
    /// <param name="contentParameters"></param>
    /// <returns></returns>
    [HttpPatch]
    [MValidationFilter]
    public async Task<IActionResult> GetRequiredContent([FromBody] List<ContentParameters> contentParameters)
    {
        var contents = await _contentService.GetRequiredContentAsync(contentParameters);

        return contents.GetObjectResponseByEntities(HttpContext, StringKey.Content, false);
    }

    /// <summary>
    /// Gets required content by entityName.
    /// </summary>
    /// <param name="entityName"></param>
    /// <param name="propName"></param>
    /// <returns></returns>
    [HttpGet("SpecMaxValue/{entityName}/{propName}")]
    [MValidateStringParameter(0, 100)]
    public async Task<IActionResult> GetMaxValueOfContent(string entityName, string propName)
    {
        var contents = await _contentService.GetSpecMaxValueAsync(entityName, propName);

        return contents.GetObjectResponseByEntity(HttpContext, StringKey.Content, false);
    }
}
