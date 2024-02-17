#region

using Scrutor.AspNetCore;

#endregion

namespace Share.Misc;

public abstract class ICancellation : IScopedLifetime {
    public abstract CancellationToken Token { get; }

    public static implicit operator CancellationToken(ICancellation m)
    {
        return m.Token;
    }
}