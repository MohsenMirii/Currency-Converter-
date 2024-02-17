

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.Misc;



namespace Share.DbContracts;

public class GenericRepo<TEntity, TDbContext> : ReadOnlyRepo<TEntity, TDbContext>, IRepo<TEntity>
    where TDbContext : DbContext
    where TEntity : class, IHasSoftDelete, new() {
    private readonly ICancellation Cancellation;

    public GenericRepo(IUnitOfWork unitOfWork
        , TDbContext dbContext
        , ICancellation cancellation
        , IServiceProvider serviceProvider)
        : base(unitOfWork, dbContext, serviceProvider, cancellation)
    {
        Cancellation = cancellation;
    }

    public UpdateBuilder<TEntity> Update()
    {
        var updateBuilder = new UpdateBuilder<TEntity>(DbContext);
        UnitOfWork.ExecuteLater(updateBuilder, DbContext);

        return updateBuilder;
    }

    public UpdateBuilder<TEntity> UpdateById(params object[] keys)
    {
        return Update().Where(GetFindByIdPredicate(keys));
    }

    public UpdateBuilder<TEntity> UpdateByUniqueKeys(params object[] keys)
    {
        return Update().Where(GetFindByUniqueKeysPredicate(keys));
    }

    public Task<List<TEntity>> SelectForUpdate(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = UnitOfWork.GetEntity<TEntity, TDbContext>(DbContext, false);

        return entity.Where(predicate).ToListAsync(Cancellation.Token);
    }

    public TEntity InitializeNewEntity(TEntity? entity = null)
    {
        entity ??= new TEntity();
        _initialize(entity);

        return entity;
    }

    public TEntity Create(TEntity entity)
    {
        _initialize(entity);

        return UnitOfWork.GetEntity<TEntity, TDbContext>(DbContext, false).Add(entity).Entity;
    }

    public void CreateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
            Create(entity);
    }

    public void DeleteById(params object[] keys)
    {
        Delete(GetFindByIdPredicate(keys));
    }

    public void DeleteByUniqueKeys(params object[] keys)
    {
        Delete(GetFindByUniqueKeysPredicate(keys));
    }

    public void Delete(Expression<Func<TEntity, bool>> predicate)
    {
        Update()
            .Where(predicate)
            .Set(r => r.IsDeleted, true)
            .Set(r => r.DeleteDateTime, DbDateTime.Now);
    }

    private void _initialize(TEntity entity)
    {
        if (entity is IHasCreateDateTime hasCreationDateTime)
            hasCreationDateTime.CreateDateTime = DbDateTime.Now;

        if (entity is IHasGuid hasGuid && hasGuid.Guid == Guid.Empty)
            hasGuid.Guid = Guid.NewGuid();

        if (entity is IHasCreatorUserId hasCreatorUserId)
            hasCreatorUserId.CreatorUserId = 1;
    }
}