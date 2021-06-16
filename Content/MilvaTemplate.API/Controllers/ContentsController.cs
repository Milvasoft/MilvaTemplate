using Fody;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers;
using Milvasoft.Helpers.Enums;
using MilvaTemplate.API.DTOs;
using MilvaTemplate.API.Helpers;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Controllers
{
    /// <summary>
    /// Provides get contents which independent anywhere.
    /// </summary>
    /// <returns></returns>
    [Route(GlobalConstants.FullRoute)]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1.0")]
    [Authorize(Roles = RoleNames.Administrator)]
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
        /// Gets required content by <paramref name="contentParameters"/>.EntityName.
        /// </summary>
        /// <param name="contentParameters"></param>
        /// <returns></returns>
        [HttpPatch]
        [MValidationFilter]
        public async Task<IActionResult> GetRequiredContent([FromBody] List<ContentParameters> contentParameters)
        {
            var errorMessage = _sharedLocalizer.GetErrorMessage("Content", CrudOperation.GetAll);
            var contents = await _contentService.GetRequiredContent(contentParameters);
            return contents.GetObjectResponse(_sharedLocalizer["SuccessfullyOperationMessage"], errorMessage);
        }

        /// <summary>
        /// Gets required content by <paramref name="entityName"/>.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        [HttpGet("SpecMaxValue/{entityName}/{propName}")]
        [MValidateStringParameter(0, 100)]
        public async Task<IActionResult> GetMaxValueOfContent(string entityName, string propName)
        {
            var errorMessage = _sharedLocalizer.GetErrorMessage("Content", CrudOperation.GetAll);
            var contents = await _contentService.GetSpecMaxValue(entityName, propName);
            return contents.GetObjectResponse(_sharedLocalizer["SuccessfullyOperationMessage"], errorMessage);
        }
    }
}
