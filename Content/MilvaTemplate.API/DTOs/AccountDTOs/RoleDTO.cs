using MilvaTemplate.API.Helpers.Attributes.ValidationAttributes;

namespace MilvaTemplate.API.DTOs.AccountDTOs;

/// <summary>
/// Role model.
/// </summary>
public class RoleDTO
{
    /// <summary>
    /// Id of Role
    /// </summary>
    [MValidateId]
    public Guid RoleId { get; set; }

    /// <summary>
    /// Name of role.
    /// </summary>
    [MValidateString(1, 500)]
    public string RoleName { get; set; }
}
