using Microsoft.Extensions.DependencyInjection;
using Milvasoft.Helpers.DataAccess.EfCore.Abstract.Entity;
using MilvaTemplate.Data;

namespace MilvaTemplate.API.Migrations;

/// <summary>
/// Defines the <see cref="DataSeed" />.
/// </summary>
[ConfigureAwait(false)]
public static class DataSeed
{
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

        foreach (var entity in entityList) //For development
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
