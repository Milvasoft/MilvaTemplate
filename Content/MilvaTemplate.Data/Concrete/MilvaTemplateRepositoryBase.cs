using Microsoft.EntityFrameworkCore;
using Milvasoft.Core;
using Milvasoft.Core.EntityBase.Abstract;
using Milvasoft.DataAccess.EfCore.IncludeLibrary;
using Milvasoft.Helpers.DataAccess.EfCore.Concrete;
using MilvaTemplate.Data.Abstract;
using MilvaTemplate.Entity.Identity;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MilvaTemplate.Data.Concrete;

/// <summary>
/// Concrete class for MilvaTemplate repository basea.
/// </summary>
public class MilvaTemplateRepositoryBase<TEntity, TKey> : BaseRepository<TEntity, TKey, MilvaTemplateDbContext>, IMilvaTemplateRepositoryBase<TEntity, TKey>
    where TEntity : class, IBaseEntity<TKey>
    where TKey : struct, IEquatable<TKey>
{
    /// <summary>
    /// Constructor of <c>BillRepository</c> class.
    /// </summary>
    /// <param name="dbContext"></param>
    public MilvaTemplateRepositoryBase(MilvaTemplateDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Returns one entity by entity Id from database asynchronously.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="conditionExpression"></param>
    /// <param name="projectionExpression"></param>
    /// <param name="tracking"></param>
    /// <returns> The entity found or null. </returns>
    public async Task<TEntity> GetByIdWithAuditAsync(TKey id,
                                                     Expression<Func<TEntity, bool>> conditionExpression = null,
                                                     Expression<Func<TEntity, TEntity>> projectionExpression = null,
                                                     bool tracking = false)
    {
        var mainCondition = CreateKeyEqualityExpression(id, conditionExpression);

        var creatorUserPredicate = SelectProperty<MilvaTemplateUser>("CreatorUser");
        var lastModifierUserPredicate = SelectProperty<MilvaTemplateUser>("LastModifierUser");

        if (creatorUserPredicate != null || lastModifierUserPredicate != null)
            return await _dbContext.Set<TEntity>().AsTracking(GetQueryTrackingBehavior(tracking)).IncludeMultiple(i => i.Include(creatorUserPredicate).Include(lastModifierUserPredicate)).Select(projectionExpression ?? (entity => entity)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
        else return await _dbContext.Set<TEntity>().AsTracking(GetQueryTrackingBehavior(tracking)).Select(projectionExpression ?? (entity => entity)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns one entity which IsDeleted condition is true by entity Id with includes from database asynchronously. If the condition is requested, it also provides that condition. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includes"></param>
    /// <param name="conditionExpression"></param>
    /// <param name="projectionExpression"></param>
    /// <param name="tracking"></param>
    /// <returns> The entity found or null. </returns>
    public async Task<TEntity> GetByIdWithAuditAsync(TKey id,
                                                    Func<IIncludable<TEntity>, IIncludable> includes,
                                                    Expression<Func<TEntity, bool>> conditionExpression = null,
                                                    Expression<Func<TEntity, TEntity>> projectionExpression = null,
                                                    bool tracking = false)
    {
        var mainCondition = CreateKeyEqualityExpression(id, conditionExpression);

        var creatorUserPredicate = SelectProperty<MilvaTemplateUser>("CreatorUser");
        var lastModifierUserPredicate = SelectProperty<MilvaTemplateUser>("LastModifierUser");

        if (creatorUserPredicate != null || lastModifierUserPredicate != null)
            return await _dbContext.Set<TEntity>().AsTracking(GetQueryTrackingBehavior(tracking)).IncludeMultiple(includes).IncludeMultiple(i => i.Include(creatorUserPredicate).Include(lastModifierUserPredicate)).Select(projectionExpression ?? (entity => entity)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
        else return await _dbContext.Set<TEntity>().AsTracking(GetQueryTrackingBehavior(tracking)).IncludeMultiple(includes).Select(projectionExpression ?? (entity => entity)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
    }

    private static Expression<Func<TEntity, TPropertyType>> SelectProperty<TPropertyType>(string propertyName)
    {
        var entityType = typeof(TEntity);

        if (!CommonHelper.PropertyExists<TEntity>(propertyName))
            return null;

        var parameter = Expression.Parameter(entityType);

        return Expression.Lambda<Func<TEntity, TPropertyType>>(Expression.Property(parameter, propertyName), parameter);
    }
}
