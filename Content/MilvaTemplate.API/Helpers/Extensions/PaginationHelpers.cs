using Milvasoft.Helpers.DataAccess.EfCore.Abstract;
using Milvasoft.Helpers.DataAccess.EfCore.Abstract.Entity;
using Milvasoft.Helpers.DataAccess.EfCore.IncludeLibrary;
using MilvaTemplate.Data;
using MilvaTemplate.Data.Abstract;

namespace MilvaTemplate.API.Helpers.Extensions;

/// <summary>
/// Helper extensions methods for Ops!yon Project.
/// </summary>
public static partial class HelperExtensions
{
    #region Pagination Helpers

    /// <summary>
    /// Prepares pagination dto according to pagination parameters.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="pageIndex"></param>
    /// <param name="requestedItemCount"></param>
    /// <param name="orderByProperty"></param>
    /// <param name="orderByAscending"></param>
    /// <param name="condition"></param>
    /// <param name="includes"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
        PreparePagination<TEntity, TKey>(this IMilvaTemplateRepositoryBase<TEntity, TKey> repository,
                                         int pageIndex,
                                         int requestedItemCount,
                                         string orderByProperty = null,
                                         bool orderByAscending = false,
                                         Expression<Func<TEntity, bool>> condition = null,
                                         Func<IIncludable<TEntity>, IIncludable> includes = null,
                                         Expression<Func<TEntity, TEntity>> projection = null)
        where TKey : struct, IEquatable<TKey>
        where TEntity : class, IBaseEntity<TKey>
        => await PreparePagination<IMilvaTemplateRepositoryBase<TEntity, TKey>, TEntity, TKey>(repository,
                                                                                               pageIndex,
                                                                                               requestedItemCount,
                                                                                               orderByProperty,
                                                                                               orderByAscending,
                                                                                               condition,
                                                                                               includes,
                                                                                               projection);

    /// <summary>
    /// Prepares pagination dto according to pagination parameters.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="pageIndex"></param>
    /// <param name="requestedItemCount"></param>
    /// <param name="orderByKeySelector"></param>
    /// <param name="orderByAscending"></param>
    /// <param name="condition"></param>
    /// <param name="includes"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
        PreparePagination<TEntity, TKey>(this IMilvaTemplateRepositoryBase<TEntity, TKey> repository,
                                         int pageIndex,
                                         int requestedItemCount,
                                         Expression<Func<TEntity, object>> orderByKeySelector = null,
                                         bool orderByAscending = false,
                                         Expression<Func<TEntity, bool>> condition = null,
                                         Func<IIncludable<TEntity>, IIncludable> includes = null,
                                         Expression<Func<TEntity, TEntity>> projection = null)
        where TKey : struct, IEquatable<TKey>
        where TEntity : class, IBaseEntity<TKey>
        => await PreparePagination<IMilvaTemplateRepositoryBase<TEntity, TKey>, TEntity, TKey>(repository,
                                                                                               pageIndex,
                                                                                               requestedItemCount,
                                                                                               orderByKeySelector,
                                                                                               orderByAscending,
                                                                                               condition,
                                                                                               includes,
                                                                                               projection);

    /// <summary>
    /// Prepares pagination dto according to pagination parameters.
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="pageIndex"></param>
    /// <param name="requestedItemCount"></param>
    /// <param name="orderByProperty"></param>
    /// <param name="orderByAscending"></param>
    /// <param name="condition"></param>
    /// <param name="includes"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
        PreparePagination<TRepository, TEntity, TKey>(this TRepository repository,
                                                      int pageIndex,
                                                      int requestedItemCount,
                                                      string orderByProperty = null,
                                                      bool orderByAscending = false,
                                                      Expression<Func<TEntity, bool>> condition = null,
                                                      Func<IIncludable<TEntity>, IIncludable> includes = null,
                                                      Expression<Func<TEntity, TEntity>> projection = null)
        where TRepository : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
        where TKey : struct, IEquatable<TKey>
        where TEntity : class, IBaseEntity<TKey>
        => string.IsNullOrWhiteSpace(orderByProperty) ? await repository.GetAsPaginatedAsync(pageIndex,
                                                                                             requestedItemCount,
                                                                                             includes,
                                                                                             condition,
                                                                                             projection)
                                                 : await repository.GetAsPaginatedAndOrderedAsync(pageIndex,
                                                                                                  requestedItemCount,
                                                                                                  includes,
                                                                                                  orderByProperty,
                                                                                                  orderByAscending,
                                                                                                  condition,
                                                                                                  projection);

    /// <summary>
    /// Prepares pagination dto according to pagination parameters.
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="repository"></param>
    /// <param name="pageIndex"></param>
    /// <param name="requestedItemCount"></param>
    /// <param name="orderByKeySelector"></param>
    /// <param name="orderByAscending"></param>
    /// <param name="condition"></param>
    /// <param name="includes"></param>
    /// <param name="projection"></param>
    /// <returns></returns>
    public static async Task<(IEnumerable<TEntity> entities, int pageCount, int totalDataCount)>
        PreparePagination<TRepository, TEntity, TKey>(this TRepository repository,
                                                      int pageIndex,
                                                      int requestedItemCount,
                                                      Expression<Func<TEntity, object>> orderByKeySelector = null,
                                                      bool orderByAscending = false,
                                                      Expression<Func<TEntity, bool>> condition = null,
                                                      Func<IIncludable<TEntity>, IIncludable> includes = null,
                                                      Expression<Func<TEntity, TEntity>> projection = null)
        where TRepository : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
        where TKey : struct, IEquatable<TKey>
        where TEntity : class, IBaseEntity<TKey>
        => orderByKeySelector == null ? await repository.GetAsPaginatedAsync(pageIndex,
                                                                             requestedItemCount,
                                                                             includes,
                                                                             condition,
                                                                             projection)
                                      : await repository.GetAsPaginatedAndOrderedAsync(pageIndex,
                                                                                       requestedItemCount,
                                                                                       includes,
                                                                                       orderByKeySelector,
                                                                                       orderByAscending,
                                                                                       condition,
                                                                                       projection);

    #endregion
}
