using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Milvasoft.Helpers.Identity.Abstract;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.Data;
using MilvaTemplate.Entity.Identity;
using MilvaTemplate.Localization;
using System;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Services.Abstract;

/// <summary>
/// The class in which user transactions are entered and exited
/// </summary>
public interface IAccountService : IIdentityOperations<UserManager<MilvaTemplateUser>, MilvaTemplateDbContext, IStringLocalizer<SharedResource>, MilvaTemplateUser, MilvaTemplateRole, Guid, LoginResultDTO>
{
    /// <summary>
    /// Login for incoming user. Returns a token if login informations are valid or the user is not lockedout. Otherwise returns the error list.
    /// </summary>
    /// <param name="loginDTO"></param>
    /// <returns></returns>
    Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO);

    /// <summary>
    /// Refresh token login for all users.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    Task<LoginResultDTO> RefreshTokenLogin(string refreshToken);
}
