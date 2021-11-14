using Milvasoft.Helpers.DataAccess.EfCore.Abstract.Entity;
using Milvasoft.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region IEnumerable Helpers

    /// <summary>
    /// Checks guid list. If list is null or empty return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDTO"></typeparam>
    /// <param name="toBeCheckedList"></param>
    /// <param name="returnFunc"></param>
    /// <returns></returns>
    public static List<TDTO> CheckGuidList<TEntity, TDTO>(this IEnumerable<TEntity> toBeCheckedList, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> returnFunc)
     where TDTO : new()
     where TEntity : class, IBaseEntity<Guid>
     => toBeCheckedList.IsNullOrEmpty() ? new List<TDTO>() : returnFunc.Invoke(toBeCheckedList).ToList();

    /// <summary>
    /// Checks int list. If list is null or empty return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDTO"></typeparam>
    /// <param name="toBeCheckedList"></param>
    /// <param name="returnFunc"></param>
    /// <returns></returns>
    public static List<TDTO> CheckIntList<TEntity, TDTO>(this IEnumerable<TEntity> toBeCheckedList, Func<IEnumerable<TEntity>, IEnumerable<TDTO>> returnFunc)
     where TDTO : new()
     where TEntity : class, IBaseEntity<int>
     => toBeCheckedList.IsNullOrEmpty() ? new List<TDTO>() : returnFunc.Invoke(toBeCheckedList).ToList();

    /// <summary>
    /// Checks guid object. If is null return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDTO"></typeparam>
    /// <param name="toBeCheckedObject"></param>
    /// <param name="returnFunc"></param>
    /// <returns></returns>
    public static TDTO CheckGuidObject<TEntity, TDTO>(this TEntity toBeCheckedObject, Func<TEntity, TDTO> returnFunc)
      where TDTO : new()
      where TEntity : class, IBaseEntity<Guid>
   => toBeCheckedObject == null ? default : returnFunc.Invoke(toBeCheckedObject);

    /// <summary>
    /// Checks int object. If is null return default(<typeparamref name="TDTO"/>). Otherwise invoke <paramref name="returnFunc"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDTO"></typeparam>
    /// <param name="toBeCheckedObject"></param>
    /// <param name="returnFunc"></param>
    /// <returns></returns>
    public static TDTO CheckIntObject<TEntity, TDTO>(this TEntity toBeCheckedObject, Func<TEntity, TDTO> returnFunc)
     where TDTO : new()
     where TEntity : class, IBaseEntity<int>
     => toBeCheckedObject == null ? default : returnFunc.Invoke(toBeCheckedObject);

    #endregion
}
