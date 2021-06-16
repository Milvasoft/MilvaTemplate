using Microsoft.AspNetCore.Identity;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilvaTemplate.Entity.Identity
{
    /// <summary>
    /// Defines the <see cref="MilvaTemplateRole" />. This user is role to authentication in MilvaTemplate api.
    /// </summary>
    [Table("MilvaTemplateRole")]
    public class MilvaTemplateRole : IdentityRole<Guid>, IFullAuditable<Guid>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public DateTime? LastModificationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid? CreatorUserId { get; set; }
        public Guid? LastModifierUserId { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleterUserId { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}