using Microsoft.AspNetCore.Identity;
using Milvasoft.Core.EntityBase.Abstract;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilvaTemplate.Entity.Identity;

/// <summary>
/// Defines the <see cref="MilvaTemplateRole" />. This user is role to authentication in MilvaTemplate api.
/// </summary>
[Table(TableNames.MilvaTemplateRole)]
public class MilvaTemplateRole : IdentityRole<Guid>, IFullAuditable<MilvaTemplateUser, Guid, Guid>
{
    #region Audit

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member   

    #endregion
}
