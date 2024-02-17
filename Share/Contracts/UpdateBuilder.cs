

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;



namespace Share.Contracts;

public class UpdateBuilder<TEntity>
    where TEntity : class {
    public UpdateBuilder(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    private DbContext DbContext { get; }

    private Expression<Func<TEntity, bool>>? Condition { get; set; }

    private List<UpdateSetter> Setters { get; } = new();

    public bool IgnoreNoSetterError { get; set; }

    public UpdateBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        Condition = predicate;

        return this;
    }

    [UsedImplicitly]
    internal void InternalWhere(string[] memberNames, object[] values)
    {
        if (memberNames.Length == 0 || memberNames.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("memberNames is not valid");

        if (values.Length == 0 || memberNames.Length != values.Length)
            throw new ArgumentException("memberNames is not valid");

        Expression? expression = null;
        var parameterExpression = Expression.Parameter(typeof(TEntity));

        for (var i = 0; i < memberNames.Length; i++)
            expression = expression == null
                ? GetExpressionByPropNameAndKeyValue(parameterExpression, memberNames[i], values[i])
                : Expression.AndAlso(expression,
                    GetExpressionByPropNameAndKeyValue(parameterExpression, memberNames[i], values[i]));

        Condition = (Expression<Func<TEntity, bool>>)Expression.Lambda(expression!, parameterExpression);

        Expression GetExpressionByPropNameAndKeyValue(Expression parameter, string propertyName, object key)
        {
            var left = Expression.Property(parameter, propertyName);
            var right = Expression.Constant(key);

            return Expression.Equal(left, right);
        }
    }

    [UsedImplicitly]
    internal void InternalSet(string memberName, object? value, Type valueType)
    {
        Setters.Add(new UpdateSetter
        {
            MemberName = memberName, Value = value, ValueType = valueType
        });
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        if (Setters.Count == 0)
            return IgnoreNoSetterError ? 0 : throw new Exception("Setters.Count == 0");

        if (Condition == null)
            throw new Exception("Condition == null");

        var rowsToUpdate = await DbContext.Set<TEntity>().Where(Condition).ToArrayAsync(cancellationToken);

        if (rowsToUpdate.Length == 0)
            return 0;

        foreach (var row in rowsToUpdate)
        {
            foreach (var setter in Setters)
                setter.Apply(row);

            DbContext.Entry(row).State = EntityState.Modified;
        }

        return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public UpdateBuilder<TEntity> Set<TValue>(string memberName, TValue? value)
    {
        Setters.Add(new UpdateSetter
        {
            MemberName = memberName, Value = value, ValueType = typeof(TValue)
        });

        return this;
    }

    public UpdateBuilder<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> memberExpression
        , Expression<Func<TEntity, TValue>> valueExpression)
    {
        Setters.Add(new UpdateSetter
        {
            MemberExpression = memberExpression, ValueExpression = valueExpression, ValueType = typeof(TValue)
        });

        return this;
    }

    public UpdateBuilder<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> member, TValue? value)
    {
        Setters.Add(new UpdateSetter
        {
            MemberExpression = member, Value = value, ValueType = typeof(TValue)
        });

        return this;
    }
}