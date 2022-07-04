using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Milvasoft.DataAccess.EfCore.Abstract;
using Milvasoft.Identity.Abstract;
using Milvasoft.Identity.Concrete;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MilvaTemplate.API.Services.Concrete;

/// <summary>
/// Provides sign-in,sign-up and sign-out process for user.
/// </summary>
[ConfigureAwait(false)]
public class AccountService : IAccountService
{
    private readonly UserManager<MilvaTemplateUser> _userManager;
    private readonly Lazy<SignInManager<MilvaTemplateUser>> _lazySignInManager;
    private readonly Lazy<RoleManager<MilvaTemplateRole>> _lazyRoleManager;
    private readonly Lazy<ITokenManagement> _lazyTokenManagement;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
    private readonly string _userName;

    /// <summary>
    /// Performs constructor injection for repository interfaces used in this service.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="lazySignInManager"></param>
    /// <param name="lazyRoleManager"></param>
    /// <param name="lazyTokenManagement"></param>
    /// <param name="contextRepository"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="sharedLocalizer"></param>
    public AccountService(UserManager<MilvaTemplateUser> userManager,
                          Lazy<SignInManager<MilvaTemplateUser>> lazySignInManager,
                          Lazy<RoleManager<MilvaTemplateRole>> lazyRoleManager,
                          Lazy<ITokenManagement> lazyTokenManagement,
                          IContextRepository<MilvaTemplateDbContext> contextRepository,
                          IHttpContextAccessor httpContextAccessor,
                          IStringLocalizer<SharedResource> sharedLocalizer)
    {
        _userName = httpContextAccessor.HttpContext.User.Identity.Name;
        _userManager = userManager;
        _lazySignInManager = lazySignInManager;
        _lazyRoleManager = lazyRoleManager;
        _lazyTokenManagement = lazyTokenManagement;
        _sharedLocalizer = sharedLocalizer;
    }

    /// <summary>
    /// Login for incoming user. Returns a token if login informations are valid or the user is not lockedout. Otherwise returns the error list.
    /// </summary>
    /// <param name="loginDTO"></param>
    /// <returns></returns>
    public async Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO)
    {
        var user = await ValidateUserAsync(loginDTO);

        var signInResult = await _lazySignInManager.Value.PasswordSignInAsync(user, loginDTO.Password, true, lockoutOnFailure: true);

        LoginResultDTO loginResult = new();

        if (signInResult.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);

            loginResult.Token = GenerateTokenWithRole(user.UserName, roles) as MilvaToken;

            return loginResult;
        }

        //If there is a failed account login, the value in the AccessFailedCount column will be increased by +1. 
        var result = await _userManager.AccessFailedAsync(user);

        result.ThrowErrorMessagesIfNotSuccess();

        if (signInResult.RequiresTwoFactor)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.RequiredTwoFactor));

        if (signInResult.IsNotAllowed)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.NotAllowed));

        return loginResult;
    }

    /// <summary>
    /// Refresh token login for all users.
    /// </summary>
    /// <param name="refreshLoginDTO"></param>
    /// <returns></returns>
    public async Task<LoginResultDTO> RefreshTokenLogin(RefreshLoginDTO refreshLoginDTO)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshLoginDTO.RefreshToken);

        if (user == null)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.TokenExpired), 31);

        var roles = await _userManager.GetRolesAsync(user);

        var token = (MilvaToken)GenerateTokenWithRole(user.UserName, roles);

        user.RefreshToken = token.RefreshToken;

        await _userManager.UpdateAsync(user);

        return new LoginResultDTO
        {
            Token = token,
        };
    }

    /// <summary>
    /// Logout process for current logged in user..
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        if (string.IsNullOrWhiteSpace(_userName))
            throw new MilvaUserFriendlyException(nameof(ResourceKey.AlreadyLoggedOutMessage));

        var user = await _userManager.FindByNameAsync(_userName);

        user.ThrowIfNullForGuidObject();

        await _lazySignInManager.Value.SignOutAsync().ConfigureAwait(false);
    }


    #region Helper Methods

    /// <summary>
    /// Defines token expired date.
    /// </summary>
    /// <returns></returns>
    private static DateTime GetTokenExpiredDate() => DateTime.UtcNow.AddDays(1);

    /// <summary>
    /// Validating user to login.
    /// </summary>
    /// <param name="loginDTO"></param>
    /// <returns></returns>
    private async Task<MilvaTemplateUser> ValidateUserAsync(LoginDTO loginDTO)
    {
        MilvaTemplateUser user = new();

        #region Common Validation For All Modules

        if (loginDTO.UserName == null)
            throw new MilvaUserFriendlyException(nameof(ResourceKey.PleaseEnterUsername));

        if (loginDTO.UserName != null)
        {
            user = await _userManager.FindByNameAsync(userName: loginDTO.UserName);

            bool userNotFound = user == null || user.IsDeleted;

            if (userNotFound)
                throw new MilvaUserFriendlyException(nameof(ResourceKey.InvalidLogin));
        }

        var userLocked = await _userManager.IsLockedOutAsync(user);

        //If the user is locked out and the unlock date has passed.
        if (userLocked && DateTime.UtcNow > user.LockoutEnd.Value.DateTime)
        {
            //_contextRepository.Value.InitializeUpdating<OpsiyonUser, Guid>(user);

            //We reset the duration of the locked user.
            await _userManager.SetLockoutEndDateAsync(user, null);

            await _userManager.ResetAccessFailedCountAsync(user);

            userLocked = false;
        }

        if (userLocked)
            ThrowIfLocked(user.LockoutEnd.Value.DateTime);

        var isPasswordTrue = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!isPasswordTrue)
        {
            //_contextRepository.Value.InitializeUpdating<OpsiyonUser, Guid>(user);

            await _userManager.AccessFailedAsync(user);

            if (await _userManager.IsLockedOutAsync(user))
                ThrowIfLocked(user.LockoutEnd.Value.DateTime);

            int accessFailedCountLimit = 5;

            throw new MilvaUserFriendlyException(messageOrLocalizerKey: nameof(ResourceKey.LockWarning), exceptionObjects: accessFailedCountLimit - user.AccessFailedCount);
        }

        #endregion

        return user;

        void ThrowIfLocked(DateTime lockoutEnd)
        {
            var remainingLockoutEnd = lockoutEnd - DateTime.UtcNow;

            var reminingLockoutEndString = remainingLockoutEnd.Hours > GlobalConstant.Zero
                                            ? _sharedLocalizer[nameof(ResourceKey.Hours), remainingLockoutEnd.Hours]
                                            : remainingLockoutEnd.Minutes > GlobalConstant.Zero
                                                 ? _sharedLocalizer[nameof(ResourceKey.Minutes), remainingLockoutEnd.Minutes]
                                                 : _sharedLocalizer[nameof(ResourceKey.Seconds), remainingLockoutEnd.Seconds];

            throw new MilvaUserFriendlyException(nameof(ResourceKey.Locked), reminingLockoutEndString);
        }
    }

    /// <summary>
    /// If Authentication is successful, JWT tokens are generated.
    /// </summary>
    private IToken GenerateTokenWithRole(string username, IList<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        //Kullanıcıya ait roller Tokene Claim olarak ekleniyor
        var claimsIdentityList = new ClaimsIdentity(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        claimsIdentityList.AddClaim(new Claim(ClaimTypes.Name, username));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentityList,
            //Issuer = _tokenManagement.Issuer,
            //Audience = _tokenManagement.Audience,
            Expires = GetTokenExpiredDate(),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_lazyTokenManagement.Value.Secret)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);//Token Üretimi

        return new MilvaToken
        {
            AccessToken = tokenHandler.WriteToken(token),
            Expiration = GetTokenExpiredDate(),
            RefreshToken = IdentityHelpers.CreateRefreshToken()
        };
    }

    private async Task<List<RoleDTO>> GetRolesByUserAsync(MilvaTemplateUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        if (userRoles.IsNullOrEmpty()) return new List<RoleDTO>();

        var accessibleRoles = await _lazyRoleManager.Value.Roles.Where(r => !r.IsDeleted && userRoles.Contains(r.Name)).ToListAsync();

        return accessibleRoles.CheckList(f => accessibleRoles.Select(r => new RoleDTO
        {
            RoleId = r.Id,
            RoleName = r.Name,
        }));
    }

    #endregion
}
