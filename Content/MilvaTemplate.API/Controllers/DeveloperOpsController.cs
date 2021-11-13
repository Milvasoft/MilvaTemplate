using Fody;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Extensions;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Helpers.Constants;
using MilvaTemplate.API.Migrations;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Entity.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Controllers;

/// <summary>
/// Provides competition's CRUD operations for admin and competition's operations for app users.
/// </summary>
[Route(GlobalConstant.FullRoute)]
[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1.0")]
//[Authorize(Roles = RoleNames.Developer)]
[ConfigureAwait(false)]
public class DeveloperOpsController : ControllerBase
{
    private readonly UserManager<MilvaTemplateUser> _userManager;
    private readonly RoleManager<MilvaTemplateRole> _roleManager;
    private readonly IAccountService _accountService;

    /// <summary>
    /// Initializes new instances of <see cref="DeveloperOpsController"/>.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="accountService"></param>
    /// <param name="roleManager"></param>
    public DeveloperOpsController(UserManager<MilvaTemplateUser> userManager,
                                  IAccountService accountService,
                                  RoleManager<MilvaTemplateRole> roleManager)
    {
        _userManager = userManager;
        _accountService = accountService;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Switch environment.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Switch/AppEnv")]
    public IActionResult SwitchAppEnv()
    {
        var oldState = GlobalConstant.RealProduction;

        GlobalConstant.RealProduction = !GlobalConstant.RealProduction;

        return Ok($"{oldState} => {GlobalConstant.RealProduction}");
    }

    #region For Development

    /// <summary>
    /// Return any user token.
    /// </summary>
    /// <returns></returns>
    [HttpGet("AnyToken")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAnyAdminToken()
    {
        var users = _userManager.Users;

        if (users.IsNullOrEmpty())
            throw new MilvaUserFriendlyException("No users found.");

        var user = users.First();

        var loginDTO = new LoginDTO
        {
            Password = $"{user.UserName}+1234",
            UserName = user.UserName
        };

        var result = await _accountService.LoginAsync(loginDTO);

        result.Token.AccessToken = $"Bearer {result.Token.AccessToken}";

        return Ok(result);
    }

    /// <summary>
    /// Reset database.
    /// </summary>
    /// <returns></returns>
    [HttpGet("Reset/Database")]
    public async Task<IActionResult> ResetEntities()
    {
        var applicationBuilder = HttpContext.RequestServices.GetRequiredService<IApplicationBuilder>();

        await applicationBuilder.SeedDatabaseAsync();

        return Ok("Database successfully reseted.");
    }

    #endregion
}
