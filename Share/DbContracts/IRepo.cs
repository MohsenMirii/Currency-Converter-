#region

using System.Linq.Expressions;
using Share.Contracts;

#endregion

namespace Share.DbContracts;

public interface IRepo<TEntity> : IReadOnlyRepo<TEntity>
    where TEntity : class, IHasSoftDelete {
    TEntity InitializeNewEntity(TEntity? entity = null);

    TEntity Create(TEntity entity);

    void CreateRange(IEnumerable<TEntity> entities);

    void DeleteById(params object[] keys);

    void DeleteByUniqueKeys(params object[] keys);

    void Delete(Expression<Func<TEntity, bool>> predicate);

    UpdateBuilder<TEntity> Update();

    UpdateBuilder<TEntity> UpdateById(params object[] keys);

    UpdateBuilder<TEntity> UpdateByUniqueKeys(params object[] keys);

    Task<List<TEntity>> SelectForUpdate(Expression<Func<TEntity, bool>> predicate);
}