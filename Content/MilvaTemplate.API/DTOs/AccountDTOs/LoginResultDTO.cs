using Microsoft.AspNetCore.Identity;
using Milvasoft.Helpers.Identity.Concrete;
using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;
using System.Collections.Generic;

namespace MilvaTemplate.API.DTOs.AccountDTOs
{
    /// <summary>
    /// Login result information.
    /// </summary>
    public class LoginResultDTO : ILoginResultDTO
    {
        /// <summary>
        /// If login not success.
        /// </summary>
        public List<IdentityError> ErrorMessages { get; set; }

        /// <summary>
        /// If login is success.
        /// </summary>
        [MValidateString(5000)]
        public string Token { get; set; }
    }
}