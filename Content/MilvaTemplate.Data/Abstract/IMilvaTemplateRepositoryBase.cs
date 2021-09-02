using Milvasoft.Helpers.DataAccess.Abstract;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using Milvasoft.Helpers.DataAccess.IncludeLibrary;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MilvaTemplate.Data.Abstract
{
    /// <summary>
    /// Interface for MilvaTemplate repository basea.
    /// </summary>
    public interface IMilvaTemplateRepositoryBase<TEntity, TKey> : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
    where TEntity : class, IBaseEntity<TKey>
    where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Returns one entity by entity Id from database asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="projectionExpression"></param>
        /// <param name="tracking"></param>
        /// <returns> The entity found or null. </returns>
        Task<TEntity> GetByIdWithAuditAsync(TKey id,
                                            Expression<Func<TEntity, bool>> conditionExpression = null,
                                            Expression<Func<TEntity, TEntity>> projectionExpression = null,
                                            bool tracking = false);

        /// <summary>
        ///  Returns one entity which IsDeleted condition is true by entity Id with includes from database asynchronously. If the condition is requested, it also provides that condition. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="projectionExpression"></param>
        /// <param name="tracking"></param>
        /// <returns> The entity found or null. </returns>
        Task<TEntity> GetByIdWithAuditAsync(TKey id,
                                            Func<IIncludable<TEntity>, IIncludable> includes,
                                            Expression<Func<TEntity, bool>> conditionExpression = null,
                                            Expression<Func<TEntity, TEntity>> projectionExpression = null,
                                            bool tracking = false);
    }
}
