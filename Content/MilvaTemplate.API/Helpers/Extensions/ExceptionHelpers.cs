using Milvasoft.Core.EntityBase.Abstract;
using Milvasoft.Core.Utils.Constants;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region Exception Helpers

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="parameterObject"/> is null.
    /// </summary>
    /// <param name="parameterObject"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfParameterIsNull(this object parameterObject, string localizerKey = null)
    {
        if (parameterObject == null)
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.NullParameter);
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfListIsNullOrEmpty(this List<object> list, string localizerKey = null)
    {
        if (list.IsNullOrEmpty())
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity);
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfParameterIsNullOrEmpty<T>(this List<T> list, string localizerKey = null) where T : IEquatable<T>
    {
        if (list.IsNullOrEmpty())
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.NullParameter);
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is null or empty.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfListIsNullOrEmpty(this IEnumerable<object> list, string localizerKey = null)
    {
        if (list.IsNullOrEmpty())
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity);
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="list"/> is not null or empty.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="message"></param>
    public static void ThrowIfListIsNotNullOrEmpty(this IEnumerable<object> list, string message = null)
    {
        if (list.IsNullOrEmpty())
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new MilvaUserFriendlyException(MilvaException.NullParameter);
            }
            else
            {
                throw new MilvaUserFriendlyException(message);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="entity"/> is null.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfNullForGuidObject<TEntity>(this TEntity entity, string localizerKey = null) where TEntity : class, IBaseEntity<Guid>
    {
        if (entity == null)
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity, $"{LocalizerKeys.LocalizedEntityName}{nameof(TEntity)}");
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    /// <summary>
    /// Throwns <see cref="MilvaUserFriendlyException"/> if <paramref name="entity"/> is null.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="localizerKey"></param>
    public static void ThrowIfNullForIntObject<TEntity>(this TEntity entity, string localizerKey = null) where TEntity : class, IBaseEntity<int>
    {
        if (entity == null)
        {
            if (string.IsNullOrWhiteSpace(localizerKey))
            {
                throw new MilvaUserFriendlyException(MilvaException.CannotFindEntity, $"{LocalizerKeys.LocalizedEntityName}{nameof(TEntity)}");
            }
            else
            {
                throw new MilvaUserFriendlyException(localizerKey);
            }
        }
    }

    #endregion
}
