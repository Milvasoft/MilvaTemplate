using Microsoft.AspNetCore.Identity;
using Milvasoft.Identity.Abstract;
using Milvasoft.Identity.Concrete;

namespace MilvaTemplate.API.DTOs.AccountDTOs;

/// <summary>
/// Login result information.
/// </summary>
public class LoginResultDTO : ILoginResultDTO<MilvaToken>
{
    /// <summary>
    /// If login not success.
    /// </summary>
    public List<IdentityError> ErrorMessages { get; set; }

    /// <summary>
    /// If login is success.
    /// </summary>
    public MilvaToken Token { get; set; }
}
