using Milvasoft.Core.EntityBase.Concrete;
using MilvaTemplate.Entity.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilvaTemplate.Entity;

/// <summary>
/// Busines Logic based information : User activity log. Which is Add-Update-Delete process.
/// </summary>
[Table(TableNames.UserActivityLog)]
public class UserActivityLog : FullAuditableEntity<MilvaTemplateUser, Guid, Guid>
{
    /// <summary>
    /// UserName of the user who performed the activity
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// User performed activity.
    /// </summary>
    public string Activity { get; set; }

}
