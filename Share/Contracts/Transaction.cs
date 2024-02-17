

using Microsoft.EntityFrameworkCore.Storage;
using Share.Misc;



namespace Share.Contracts;

public sealed class Transaction : IAsyncDisposable {
    private readonly ICancellation _cancellation;
    private readonly IDbContextTransaction _transaction;
    private int _transactionCount = 1;

    public Transaction(IDbContextTransaction transaction, ICancellation cancellation)
    {
        _transaction = transaction;
        _cancellation = cancellation;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transactionCount == 0)
            return;

        // todo: log rollback
        await _transaction.RollbackAsync(_cancellation.Token);
        await _transaction.DisposeAsync();
    }

    internal void IncreaseTransactionCount()
    {
        _transactionCount++;
    }

    public async Task CommitAsync()
    {
        switch (_transactionCount)
        {
            case > 1:
                _transactionCount--;

                break;

            case 0:
                throw new Exception("_transactionCount == 0");

            default:
                await _transaction.CommitAsync(_cancellation.Token);
                _transactionCount--;

                break;
        }
    }
}