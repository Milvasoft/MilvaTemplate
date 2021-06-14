using Microsoft.AspNetCore.Identity;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using System;

namespace MilvaTemplate.Entity.Identity
{
    /// <summary>
    /// Defines the <see cref="MilvaTemplateUser" />. This user is used to authentication in MilvaTemplate api.
    /// </summary>
    public class MilvaTemplateUser : IdentityUser<Guid>, IFullAuditable<MilvaTemplateUser, Guid, Guid>
    {
        public DateTime? LastModificationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid? CreatorUserId { get; set; }
        public Guid? LastModifierUserId { get; set; }
        public DateTime? DeletionDate { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? DeleterUserId { get; set; }
        public virtual MilvaTemplateUser DeleterUser { get; set; }
        public virtual MilvaTemplateUser LastModifierUser { get; set; }
        public virtual MilvaTemplateUser CreatorUser { get; set; }
    }
}
