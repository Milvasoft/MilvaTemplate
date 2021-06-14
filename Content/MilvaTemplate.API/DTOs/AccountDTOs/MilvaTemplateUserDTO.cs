using Milvasoft.Helpers.DataAccess.Concrete.Entity;
using System;

namespace MilvaTemplate.API.DTOs.AccountDTOs
{
    /// <summary>
    /// MilvaTemplate user.
    /// </summary>
    public class MilvaTemplateUserDTO : BaseEntity<Guid>
    {
        /// <summary>
        /// UserName of MilvaTemplate user.
        /// </summary>
        public string UserName { get; set; }

    }
}
