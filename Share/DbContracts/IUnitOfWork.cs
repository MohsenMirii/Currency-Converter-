#region

using Microsoft.EntityFrameworkCore;
using Scrutor.AspNetCore;
using Share.Contracts;

#endregion

namespace Share.DbContracts;

public interface IUnitOfWork : IAsyncDisposable, IScopedLifetime {
    internal void ExecuteLater<TEntity, TDbContext>(UpdateBuilder<TEntity> updateBuilder, TDbContext dbContext)
        where TEntity : class
        where TDbContext : DbContext;

    internal DbSet<TEntity> GetEntity<TEntity, TDbContext>(TDbContext dbContext, bool readOnly)
        where TEntity : class
        where TDbContext : DbContext;

    Task<int> SaveChangesAsync();

    Task<int> EnsureSaveChangesAsync();

    Task<Transaction> BeginTransaction<TDbContext>()
        where TDbContext : DbContext;
}