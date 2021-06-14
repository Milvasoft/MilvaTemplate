using Microsoft.EntityFrameworkCore;
using Milvasoft.Helpers;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using Milvasoft.Helpers.DataAccess.Concrete;
using Milvasoft.Helpers.DataAccess.IncludeLibrary;
using MilvaTemplate.Data.Abstract;
using MilvaTemplate.Entity.Identity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MilvaTemplate.Data.Concrete
{
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
        /// <returns> The entity found or null. </returns>
        public override async Task<TEntity> GetByIdAsync(TKey id, Expression<Func<TEntity, bool>> conditionExpression = null)
        {
            var mainCondition = CreateKeyEqualityExpression(id, conditionExpression);

            var creatorUserPredicate = SelectProperty<MilvaTemplateUser>("CreatorUser");
            var lastModifierUserPredicate = SelectProperty<MilvaTemplateUser>("LastModifierUser");

            if (creatorUserPredicate != null || lastModifierUserPredicate != null)
                return await _dbContext.Set<TEntity>().IncludeMultiple(i => i.Include(creatorUserPredicate).Include(lastModifierUserPredicate)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
            else return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
        }


        /// <summary>
        /// Returns one entity which IsDeleted condition is true by entity Id with includes from database asynchronously. If the condition is requested, it also provides that condition. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <param name="conditionExpression"></param>
        /// <returns> The entity found or null. </returns>
        public override async Task<TEntity> GetByIdAsync(TKey id,
                                                        Func<IIncludable<TEntity>, IIncludable> includes,
                                                        Expression<Func<TEntity, bool>> conditionExpression = null)
        {
            var mainCondition = CreateKeyEqualityExpression(id, conditionExpression);

            var creatorUserPredicate = SelectProperty<MilvaTemplateUser>("CreatorUser");
            var lastModifierUserPredicate = SelectProperty<MilvaTemplateUser>("LastModifierUser");

            if (creatorUserPredicate != null || lastModifierUserPredicate != null)
                return await _dbContext.Set<TEntity>().IncludeMultiple(includes).IncludeMultiple(i => i.Include(creatorUserPredicate).Include(lastModifierUserPredicate)).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
            else return await _dbContext.Set<TEntity>().IncludeMultiple(includes).SingleOrDefaultAsync(mainCondition).ConfigureAwait(false);
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
}
