using Fody;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.DataAccess.Abstract;
using Milvasoft.Helpers.Exceptions;
using Milvasoft.Helpers.Identity.Concrete;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.API.Services.Abstract;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;
using MilvaTemplate.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Services.Concrete
{
    /// <summary>
    /// Provides sign-in,sign-up and sign-out process for user.
    /// </summary>
    [ConfigureAwait(false)]
    public class AccountService : IdentityOperations<UserManager<MilvaTemplateUser>, MilvaTemplateDbContext, IStringLocalizer<SharedResource>, MilvaTemplateUser, MilvaTemplateRole, Guid, LoginResultDTO>, IAccountService
    {
        private readonly UserManager<MilvaTemplateUser> _userManager;
        private readonly SignInManager<MilvaTemplateUser> _signInManager;

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
                loginResult.Token = await GenerateTokenWithRoleAsync(user: user, tokenExpireDate);

                return loginResult;
            }

            #region Error Handling

            //Eğer ki başarısız bir account girişi söz konusu ise AccessFailedCount kolonundaki değer +1 arttırılacaktır. 
            await _userManager.AccessFailedAsync(user);

            if (signInResult.RequiresTwoFactor)
                throw new MilvaUserFriendlyException("IdentityRequiresTwoFactor");

            if (signInResult.IsNotAllowed)
                throw new MilvaUserFriendlyException("IdentityNotAllowed");

            #endregion

            return loginResult;
        }

        /// <summary>
        /// Logout process for current logged in user..
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync()
        {
            var logoutResult = await base.LogoutAsync();

            logoutResult.ThrowErrorMessagesIfNotSuccess();
        }
    }
}