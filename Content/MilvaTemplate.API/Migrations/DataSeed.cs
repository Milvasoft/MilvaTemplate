using Fody;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using MilvaTemplate.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilvaTemplate.API.Migrations
{
    /// <summary>
    /// Defines the <see cref="DataSeed" />.
    /// </summary>
    [ConfigureAwait(false)]
    public static class DataSeed
    {
        /// <summary>
        /// Resets the tables data even if table is not empty when any method called.
        /// </summary>
        public static bool ResetAnyway { get; set; } = false;
        /// <summary>
        /// Contains dipendency injection services.
        /// </summary>
        public static IServiceProvider Services { get; set; }

        /// <summary>
        /// Resets the data of type of T. This method is only calls from Reset extension method of indelible tables.
        /// </summary>
        /// <param name="entityList">The entityList<see cref="List{TEntity}"/>.</param>
        private static async Task InitializeTableAsync<TEntity, TKey>(List<TEntity> entityList) where TEntity : class, IBaseEntity<TKey>
                                                                                                where TKey : struct, IEquatable<TKey>
        {
            var dbContext = Services.GetRequiredService<MilvaTemplateDbContext>();

            foreach (var entity in dbContext.Set<TEntity>().AsNoTracking().IgnoreQueryFilters())
                dbContext.Entry(entity).State = EntityState.Detached;

            foreach (var entity in dbContext.Set<TEntity>().AsNoTracking().IgnoreQueryFilters())
                dbContext.Remove(entity);

            MilvaTemplateDbContext.IgnoreSoftDeleteForNextProcess();

            await dbContext.SaveChangesAsync();

            foreach (var entity in dbContext.Set<TEntity>().AsNoTracking().IgnoreQueryFilters())
                dbContext.Entry(entity).State = EntityState.Detached;

            await dbContext.Set<TEntity>().AddRangeAsync(entityList);

            await dbContext.SaveChangesAsync();
        }
    }
}
