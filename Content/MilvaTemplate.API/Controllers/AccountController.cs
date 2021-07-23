using Fody;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers;
using Milvasoft.Helpers.Extensions;
using Milvasoft.Helpers.Identity.Concrete;
using Milvasoft.Helpers.Models.Response;
using Milvasoft.Helpers.Utils;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Helpers;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Localization;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Controllers
{
    /// <summary>
    /// This controller uses the properties of the .NET core Identity library . We can call this the controller where API's lease agreements are made.
    /// </summary>
    /// 
    /// <remarks>
    /// <para><b>EN:</b> </para>
    /// <para> This controller uses the properties of the .NET core Identity library . We can call this the controller where API's lease agreements are made.</para> 
    /// <br></br>
    /// <para><b>TR:</b></para>
    /// <para>Ops!yon projesinin giriş çıkış (Login Logout) işlemleri ile ilgili yönetimini sağlar.</para>
    /// 
    /// </remarks>
    /// 
    /// <returns></returns>
    [Route(GlobalConstants.FullRoute)]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1.0")]
    [ConfigureAwait(false)]
    public class AccountController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IAccountService _accountService;

        /// <summary>
        /// Constructor of <c>AccountController</c>
        /// </summary>
        /// <param name="sharedLocalizer"></param>
        /// <param name="accountService"></param>
        public AccountController(IStringLocalizer<SharedResource> sharedLocalizer, IAccountService accountService)
        {
            _sharedLocalizer = sharedLocalizer;
            _accountService = accountService;

        }

        /// <summary>
        /// Login method. This endpoint is accessible for anyone.
        /// </summary>
        /// <returns></returns>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        [MValidationFilter]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            return await _accountService.LoginAsync(loginDTO).GetObjectResponseAsync(_sharedLocalizer["SuccessfullyLoginMessage"]);
        }

        /// <summary>
        /// Refresh token login for all users.
        /// </summary>
        /// <param name="refreshLogin"></param>
        /// <returns></returns>
        [HttpPost("Login/{*refreshLogin}")]
        [AllowAnonymous]
        [MValidateStringParameter(10, 1000)]
        public async Task<IActionResult> RefreshTokenLogin(string refreshLogin)
        {
            ObjectResponse<LoginResultDTO> response = new()
            {
                Result = await _accountService.RefreshTokenLogin(refreshLogin)
            };

            if (!response.Result.ErrorMessages.IsNullOrEmpty())
            {

                response.Message = response.Result.ErrorMessages.DescriptionJoin();

                response.StatusCode = MilvaStatusCodes.Status401Unauthorized;
                response.Success = false;
            }
            else if (response.Result.Token == null)
            {
                response.Message = _sharedLocalizer["UnknownLoginProblemMessage"];
                response.StatusCode = MilvaStatusCodes.Status400BadRequest;
                response.Success = false;
            }
            else
            {
                response.Message = _sharedLocalizer["SuccessfullyLoginMessage"];
                response.StatusCode = MilvaStatusCodes.Status200OK;
                response.Success = true;
            }

            return Ok(response);
        }

        /// <summary>
        /// Logout method. This endpoint is accessible for anyone.
        /// </summary>
        /// <returns></returns>
        [HttpGet("LogOut")]
        [Authorize(Roles = RoleNames.Administrator)]
        [ApiVersion("1.1")]
        [ApiExplorerSettings(GroupName = "v1.1")]
        public async Task<IActionResult> LogoutAsync()
        {
            return await _accountService.LogoutAsync().GetObjectResponseAsync<object>(_sharedLocalizer["SuccessfullyLoguotMessage"]);
        }
    }
}