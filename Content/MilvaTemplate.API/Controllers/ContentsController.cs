using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.Enums;
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
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
    private readonly IContentService _contentService;

    /// <summary>
    /// Constructor of <c>ContentsController</c>
    /// </summary>
    /// <param name="sharedLocalizer"></param>
    /// <param name="contentService"></param>
    public ContentsController(IStringLocalizer<SharedResource> sharedLocalizer, IContentService contentService)
    {
        _sharedLocalizer = sharedLocalizer;
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

        var errorMessage = _sharedLocalizer.GetErrorMessage(MilvaTemplateStringKey.Content, CrudOperation.GetAll);

        return contents.GetObjectResponse(_sharedLocalizer[nameof(ResourceKey.SuccessfullyOperationMessage)], errorMessage);
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

        var errorMessage = _sharedLocalizer.GetErrorMessage(MilvaTemplateStringKey.Content, CrudOperation.GetAll);

        return contents.GetObjectResponse(_sharedLocalizer[nameof(ResourceKey.SuccessfullyOperationMessage)], errorMessage);
    }
}
