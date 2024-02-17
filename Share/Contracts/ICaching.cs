namespace Share.Contracts;

public interface ICaching<T> {
    Task<T> GetOrSet(CancellationToken cancellationToken);
}