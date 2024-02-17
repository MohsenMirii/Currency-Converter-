

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Share.Contracts;
using Share.Misc;



namespace Share.DbContracts;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class UnitOfWork : IUnitOfWork {
    public static bool IsInMemoryDb;
    private readonly ICancellation _cancellation;
    private readonly List<Func<CancellationToken, Task<int>>> _pendingBatchUpdates = new();
    private readonly IServiceProvider _serviceProvider;
    private DbContext? _defaultDb;
    private int _saveChangeCalledCount;
    private Transaction? _transaction;

    public UnitOfWork(ICancellation cancellation, IServiceProvider serviceProvider)
    {
        _cancellation = cancellation;
        _serviceProvider = serviceProvider;
    }

    void IUnitOfWork.ExecuteLater<TEntity, TDbContext>(UpdateBuilder<TEntity> updateBuilder, TDbContext dbContext)
    {
        if (_defaultDb == null)
            _defaultDb = dbContext;
        else if (_defaultDb is not TDbContext)
            throw new Exception("Cannot use multiple db context at a scope");

        _pendingBatchUpdates.Add(updateBuilder.ExecuteAsync);
    }

    DbSet<TEntity> IUnitOfWork.GetEntity<TEntity, TDbContext>(TDbContext dbContext, bool readOnly)
    {
        if (readOnly)
            return dbContext.Set<TEntity>();

        if (_defaultDb == null)
            _defaultDb = dbContext;
        else if (_defaultDb is not TDbContext)
            throw new Exception("Cannot use multiple db context at a scope");

        return dbContext.Set<TEntity>();
    }

    public async Task<int> SaveChangesAsync()
    {
        if (_defaultDb == null)
            throw new NullReferenceException(nameof(_defaultDb));

        if (_pendingBatchUpdates.Count == 0)
        {
            var rowsAffected = await _defaultDb.SaveChangesAsync(_cancellation.Token);
            _defaultDb = null;

            return rowsAffected;
        }

        // todo: begin transaction

        var affectedRows = 0;
        var executedItems = new List<Func<CancellationToken, Task<int>>>();

        foreach (var pendingBatchUpdate in _pendingBatchUpdates)
        {
            affectedRows += await pendingBatchUpdate.Invoke(_cancellation.Token);
            executedItems.Add(pendingBatchUpdate);
        }

        _saveChangeCalledCount++;

        affectedRows += await _defaultDb.SaveChangesAsync(_cancellation.Token);

        // cleanup
        foreach (var item in executedItems)
            _pendingBatchUpdates.Remove(item);

        _defaultDb = null;

        return affectedRows;
    }

    public async Task<int> EnsureSaveChangesAsync()
    {
        var affectedRows = await SaveChangesAsync();

        if (affectedRows == 0)
            throw new Exception("affectedRows == 0");

        return affectedRows;
    }

    public async ValueTask DisposeAsync()
    {
        if (_defaultDb == null)
            return;

        await _defaultDb.DisposeAsync();
    }

    public async Task<Transaction> BeginTransaction<TDbContext>()
        where TDbContext : DbContext
    {
        if (_transaction != null)
        {
            if (_defaultDb is not TDbContext)
                throw new Exception("Cannot use multiple db context at a scope");

            _transaction.IncreaseTransactionCount();

            return _transaction;
        }

        _saveChangeCalledCount = int.MinValue;

        var dbContext = _serviceProvider.GetRequiredService<TDbContext>();

        if (_defaultDb == null)
            _defaultDb = dbContext;
        else if (_defaultDb is not TDbContext)
            throw new Exception("Cannot use multiple db context at a scope");

        var transaction = await _defaultDb.Database.BeginTransactionAsync(_cancellation.Token);

        return _transaction = new Transaction(transaction, _cancellation);
    }
}