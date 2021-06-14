using Microsoft.AspNetCore.Identity;
using Milvasoft.Helpers.Identity.Concrete;
using MilvaTemplate.API.Attributes.ValidationAttributes;
using System.Collections.Generic;

namespace MilvaTemplate.API.DTOs.AccountDTOs
{
    public class LoginResultDTO : ILoginResultDTO
    {
        public List<IdentityError> ErrorMessages { get; set; }

        [MValidateString(5000)]
        public string Token { get; set; }
    }
}