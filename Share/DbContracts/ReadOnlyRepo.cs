

using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Share.Misc;



namespace Share.DbContracts;

public class ReadOnlyRepo<TEntity, TDbContext> : IReadOnlyRepo<TEntity>
    where TDbContext : DbContext
    where TEntity : class {
    private readonly ICancellation _cancellation;
    private readonly IServiceProvider _serviceProvider;
    protected readonly TDbContext DbContext;
    protected readonly IUnitOfWork UnitOfWork;

    [UsedImplicitly]
    public ReadOnlyRepo(IUnitOfWork unitOfWork
        , TDbContext dbContext
        , IServiceProvider serviceProvider
        , ICancellation cancellation)
    {
        UnitOfWork = unitOfWork;
        DbContext = dbContext;
        _serviceProvider = serviceProvider;
        _cancellation = cancellation;
    }

    public string[] GetPrimaryKeys()
    {
        // todo: use caching
        return DbContext.Model
            .FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties
            .Select(x => x.Name)
            .ToArray();
    }

    public Expression<Func<TEntity, bool>> GetFindByIdPredicate(params object[] keys)
    {
        if (keys.Length == 0)
            throw new ArgumentException("keys.Length == 0", nameof(keys));

        // todo: use caching
        var members = DbContext.Model
            .FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties
            .Select(x => x.Name)
            .ToArray();

        if (keys.Length != members.Length)
            throw new ArgumentException($"keys.Length({keys.Length}) != members.Length(members.Length)", nameof(keys));

        Expression? expression = null;
        var parameterExpression = Expression.Parameter(typeof(TEntity));

        for (var i = 0; i < members.Length; i++)
            expression = expression == null
                ? GetExpressionByPropNameAndKeyValue(parameterExpression, members[i], keys[i])
                : Expression.AndAlso(expression,
                    GetExpressionByPropNameAndKeyValue(parameterExpression, members[i], keys[i]));

        return (Expression<Func<TEntity, bool>>)Expression.Lambda(expression!, parameterExpression);

        Expression GetExpressionByPropNameAndKeyValue(Expression parameter, string propertyName, object key)
        {
            var left = Expression.Property(parameter, propertyName);
            var right = Expression.Constant(key);

            return Expression.Equal(left, right);
        }
    }

    public IQueryable<TEntity> QueryById(params object[] keys)
    {
        return Query(GetFindByIdPredicate(keys));
    }

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
    {
        return Query().AsNoTracking().Where(expression);
    }

    public IQueryable<TEntity> Query()
    {
        return UnitOfWork.GetEntity<TEntity, TDbContext>(DbContext, true).AsNoTracking();
    }

    public Expression<Func<TEntity, bool>> GetFindByUniqueKeysPredicate(params object[] keys)
    {
        if (keys.Length == 0)
            throw new ArgumentException("keys.Length == 0", nameof(keys));

        // todo: use caching
        var members = typeof(TEntity)
            .GetProperties()
            .Select(r => new
            {
                PropertyInfo = r, UniqueKeysAttribute = r.GetCustomAttribute<UniqueKeysAttribute>()
            })
            .Where(r => r.UniqueKeysAttribute is not null)
            .Select(q => q.PropertyInfo.Name)
            .ToArray();

        if (keys.Length != members.Length)
            throw new ArgumentException($"keys.Length({keys.Length}) != members.Length(members.Length)", nameof(keys));

        Expression? expression = null;
        var parameterExpression = Expression.Parameter(typeof(TEntity));

        for (var i = 0; i < members.Length; i++)
            expression = expression == null
                ? GetExpressionByPropNameAndKeyValue(parameterExpression, members[i], keys[i])
                : Expression.AndAlso(expression,
                    GetExpressionByPropNameAndKeyValue(parameterExpression, members[i], keys[i]));

        return (Expression<Func<TEntity, bool>>)Expression.Lambda(expression!, parameterExpression);

        Expression GetExpressionByPropNameAndKeyValue(Expression parameter, string propertyName, object key)
        {
            var left = Expression.Property(parameter, propertyName);
            var right = Expression.Constant(key);

            return Expression.Equal(left, right);
        }
    }

    public Task<ODataQueryOut<TEntity>> AsOData(ODataQueryIn input, Expression<Func<TEntity, bool>>? preFilter = null)
    {
        var query = preFilter == null ? Query() : Query(preFilter);

        return query.AsOData<TEntity>(input, _cancellation.Token);
    }
}