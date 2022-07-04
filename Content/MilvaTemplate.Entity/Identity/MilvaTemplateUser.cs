using Microsoft.AspNetCore.Identity;
using Milvasoft.Core.EntityBase.Abstract;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilvaTemplate.Entity.Identity;

/// <summary>
/// Defines the <see cref="MilvaTemplateUser" />. This user is used to authentication in MilvaTemplate api.
/// </summary>
[Table(TableNames.MilvaTemplateUser)]
public class MilvaTemplateUser : IdentityUser<Guid>, IFullAuditable<MilvaTemplateUser, Guid, Guid>
{
    /// <summary>
    /// Refresh token of user.
    /// </summary>
    public string RefreshToken { get; set; }

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
