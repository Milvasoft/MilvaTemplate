using Microsoft.AspNetCore.Identity;
using Milvasoft.Helpers.Identity.Concrete;
using System.Collections.Generic;

namespace MilvaTemplate.API.DTOs.AccountDTOs
{
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
}