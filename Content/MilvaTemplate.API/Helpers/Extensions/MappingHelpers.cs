using Milvasoft.Core.EntityBase.Abstract;
using Milvasoft.Core.EntityBase.Concrete;
using MilvaTemplate.API.DTOs.AccountDTOs;
using MilvaTemplate.Entity.Identity;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region Mapping Helpers

    /// <summary>
    /// Getting cretion user datas.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static TDTO MapAuditData<TDTO, TEntity>(this TDTO dto, TEntity entity)
        where TDTO : AuditableEntityWithCustomUser<MilvaTemplateUserDTO, Guid, Guid>
        where TEntity : IAuditable<MilvaTemplateUser, Guid, Guid>
    {
        dto.CreationDate = entity.CreationDate;
        dto.CreatorUser = new MilvaTemplateUserDTO
        {
            Id = entity.CreatorUser?.Id ?? default,
            UserName = entity.CreatorUser?.UserName ?? string.Empty
        };
        dto.LastModificationDate = entity.LastModificationDate;
        dto.LastModifierUser = new MilvaTemplateUserDTO
        {
            Id = entity.LastModifierUser?.Id ?? default,
            UserName = entity.LastModifierUser?.UserName ?? string.Empty
        };
        return dto;
    }

    /// <summary>
    /// Getting cretion user datas.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static TDTO MapAuditDataForIntTypes<TDTO, TEntity>(this TDTO dto, TEntity entity)
        where TDTO : AuditableEntityWithCustomUser<MilvaTemplateUserDTO, Guid, int>
        where TEntity : IAuditable<MilvaTemplateUser, Guid, int>
    {
        dto.CreationDate = entity.CreationDate;
        dto.CreatorUser = new MilvaTemplateUserDTO
        {
            Id = entity.CreatorUser?.Id ?? default,
            UserName = entity.CreatorUser?.UserName ?? string.Empty
        };
        dto.LastModificationDate = entity.LastModificationDate;
        dto.LastModifierUser = new MilvaTemplateUserDTO
        {
            Id = entity.LastModifierUser?.Id ?? default,
            UserName = entity.LastModifierUser?.UserName ?? string.Empty
        };
        return dto;
    }

    #endregion
}
