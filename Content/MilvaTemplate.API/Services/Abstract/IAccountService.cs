using MilvaTemplate.API.DTOs.AccountDTOs;

namespace MilvaTemplate.API.Services.Abstract;

/// <summary>
/// The class in which user transactions are entered and exited
/// </summary>
public interface IAccountService
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
    /// <param name="refreshLoginDTO"></param>
    /// <returns></returns>
    Task<LoginResultDTO> RefreshTokenLogin(RefreshLoginDTO refreshLoginDTO);

    /// <summary>
    /// Logout process for current logged in user..
    /// </summary>
    /// <returns></returns>
    Task LogoutAsync();
}
