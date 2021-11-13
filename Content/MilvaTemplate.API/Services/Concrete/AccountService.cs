using Fody;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.DataAccess.EfCore.Abstract;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Identity.Abstract;
using Milvasoft.Helpers.Identity.Concrete;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;
using MilvaTemplate.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceKey = MilvaTemplate.Localization.Resources.SharedResource;

namespace MilvaTemplate.API.Services.Concrete;

/// <summary>
/// Provides sign-in,sign-up and sign-out process for user.
/// </summary>
[ConfigureAwait(false)]
public class AccountService : IdentityOperations<UserManager<MilvaTemplateUser>, MilvaTemplateDbContext, IStringLocalizer<SharedResource>, MilvaTemplateUser, MilvaTemplateRole, Guid, LoginResultDTO>, IAccountService
{
    private readonly UserManager<MilvaTemplateUser> _userManager;
    private readonly SignInManager<MilvaTemplateUser> _signInManager;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

    /// <summary>
    /// Performs constructor injection for repository interfaces used in this service.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="tokenManagement"></param>
    /// <param name="contextRepository"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="sharedLocalizer"></param>
    public AccountService(UserManager<MilvaTemplateUser> userManager,
                          SignInManager<MilvaTemplateUser> signInManager,
                          ITokenManagement tokenManagement,
                          IContextRepository<MilvaTemplateDbContext> contextRepository,
                          IHttpContextAccessor httpContextAccessor,
                          IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager,
                                                                                         signInManager,
                                                                                         tokenManagement,
                                                                                         contextRepository,
                                                                                         sharedLocalizer,
                                                                                         httpContextAccessor,
                                                                                         false)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _sharedLocalizer = sharedLocalizer;
    }

    /// <summary>
    /// Login for incoming user. Returns a token if login informations are valid or the user is not lockedout. Otherwise returns the error list.
    /// </summary>
    /// <param name="loginDTO"></param>
    /// <returns></returns>
    public async Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO)
    {
        MilvaTemplateUser user = new();

        var (tUser, loginResult) = await ValidateUserAsync(loginDTO, user);

        user = tUser;

        if (loginResult.ErrorMessages.Count > 0)
            throw new MilvaUserFriendlyException(string.Join('~', loginResult.ErrorMessages.Select(i => i.Description)));

        var tokenExpireDate = DateTime.Now.AddDays(1);

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.Persistent, lockoutOnFailure: true);

        //Kimlik doğrulama başarılı ise
        if (signInResult.Succeeded)
        {
            loginResult.Token = (MilvaToken)await GenerateTokenWithRoleAsync(user: user, tokenExpireDate);

            return loginResult;
        }

        #region Error Handling

        //Eğer ki başarısız bir account girişi söz konusu ise AccessFailedCount kolonundaki değer +1 arttırılacaktır. 
        await _userManager.AccessFailedAsync(user);

        if (signInResult.RequiresTwoFactor)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.RequiresTwoFactor));

        if (signInResult.IsNotAllowed)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.NotAllowed));

        #endregion

        return loginResult;
    }

    /// <summary>
    /// Refresh token login for all users.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public async Task<LoginResultDTO> RefreshTokenLogin(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user != null)
        {
            var token = (MilvaToken)await GenerateTokenWithRoleAsync(user: user, DateTime.Now.AddHours(12));

            user.RefreshToken = token.RefreshToken;

            await _userManager.UpdateAsync(user);

            return new LoginResultDTO
            {
                Token = token,
            };
        }
        return new LoginResultDTO
        {
            ErrorMessages = new List<IdentityError>() { new IdentityError { Code = nameof(ResourceKey.TokenExpired), Description = _sharedLocalizer[nameof(ResourceKey.TokenExpired)] } }
        };
    }

    /// <summary>
    /// Logout process for current logged in user..
    /// </summary>
    /// <returns></returns>
    public new async Task LogoutAsync()
    {
        var logoutResult = await base.LogoutAsync();

        logoutResult.ThrowErrorMessagesIfNotSuccess();
    }
}
