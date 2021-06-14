﻿using Milvasoft.Helpers.Attributes.Validation;
using Milvasoft.Helpers.Identity.Abstract;
using MilvaTemplate.API.Attributes.ValidationAttributes;
using MilvaTemplate.Localization;

namespace MilvaTemplate.API.DTOs.AccountDTOs
{
    /// <summary>
    /// Login and sign up processes are happens with this dto.
    /// </summary>
    public class LoginDTO : ILoginDTO
    {
        /// <summary>
        /// UserName of user.
        /// </summary>
        [MValidateString(0, 20)]
        public string UserName { get; set; }

        /// <summary>
        /// Email of user.
        /// </summary>
        [MValidateString(0, 100)]
        [MilvaRegex(typeof(SharedResource), IsRequired = false)]
        public string Email { get; set; }

        /// <summary>
        /// Password of user.
        /// </summary>
        [MValidateString(2, 250)]
        public string Password { get; set; }

        /// <summary>
        /// Remember me option.
        /// </summary>
        public bool Persistent { get; set; }
    }
}
