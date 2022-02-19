namespace MilvaTemplate.Entity;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
/// <summary>
/// The reason this class has a nameof(Entity.z) at the beginning of the filename is because we want it to located at the very end of folder.
/// </summary>
public static class TableNames
{
    public const string MilvaTemplateUser = nameof(Identity.MilvaTemplateUser);
    public const string MilvaTemplateRole = nameof(Identity.MilvaTemplateRole);
    public const string SystemLanguage = nameof(Entity.SystemLanguage);
    public const string UserActivityLog = nameof(Entity.UserActivityLog);
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
