#region

using System.Linq.Expressions;

#endregion

namespace Share.DbContracts;

public interface IReadOnlyRepo<TEntity> {
    string[] GetPrimaryKeys();

    Expression<Func<TEntity, bool>> GetFindByIdPredicate(params object[] keys);

    IQueryable<TEntity> Query();

    IQueryable<TEntity> QueryById(params object[] keys);

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression);

    // todo: Task<ODataQueryOut<TEntity>> AsOData(ODataQueryIn input, Expression<Func<TEntity, bool>>? preFilter = null);
}