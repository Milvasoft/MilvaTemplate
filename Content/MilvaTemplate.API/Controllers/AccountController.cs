using Fody;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers;
using MilvaTemplate.API.Attributes.ActionFilters;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Helpers;
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