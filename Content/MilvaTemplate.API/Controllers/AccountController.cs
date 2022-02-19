using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Helpers.Attributes.ActionFilters;
using MilvaTemplate.API.Services.Abstract;

namespace MilvaTemplate.API.Controllers;

/// <summary>
/// This controller uses the properties of the .NET core Identity library. We can call this the controller where API's lease agreements are made.
/// </summary>
/// <returns></returns>
[Route(GlobalConstant.FullRoute)]
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
        var loginResult = await _accountService.LoginAsync(loginDTO);

        return loginResult.GetObjectResponse(_sharedLocalizer[nameof(ResourceKey.SuccessfullyLoginMessage)]);
    }

    /// <summary>
    /// Refresh token login for all users.
    /// </summary>
    /// <param name="refreshLoginDTO"></param>
    /// <returns></returns>
    [HttpPost("Login/{*refreshLogin}")]
    [AllowAnonymous]
    [MValidateStringParameter(10, 1000)]
    public async Task<IActionResult> RefreshTokenLogin(RefreshLoginDTO refreshLoginDTO)
    {
        var loginResult = await _accountService.RefreshTokenLogin(refreshLoginDTO);

        return loginResult.GetObjectResponse(_sharedLocalizer[nameof(ResourceKey.SuccessfullyLoginMessage)]);
    }

    /// <summary>
    /// Logout method. This endpoint is accessible for anyone.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Logout")]
    [Authorize(Roles = RoleName.All)]
    public async Task<IActionResult> LogoutAsync()
        => await _accountService.LogoutAsync().GetObjectResponseAsync<object>(_sharedLocalizer[nameof(ResourceKey.SuccessfullyLoguotMessage)]);
}
