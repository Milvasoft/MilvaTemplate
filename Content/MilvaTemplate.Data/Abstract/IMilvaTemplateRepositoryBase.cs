using Milvasoft.Helpers.DataAccess.Abstract;
using Milvasoft.Helpers.DataAccess.Abstract.Entity;
using System;

namespace MilvaTemplate.Data.Abstract
{
    /// <summary>
    /// Interface for MilvaTemplate repository basea.
    /// </summary>
    public interface IMilvaTemplateRepositoryBase<TEntity, TKey> : IBaseRepository<TEntity, TKey, MilvaTemplateDbContext>
    where TEntity : class, IBaseEntity<TKey>
    where TKey : struct, IEquatable<TKey>
    {
    }
}
