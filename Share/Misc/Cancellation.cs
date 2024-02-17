

using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;



namespace Share.Misc;

[UsedImplicitly]
public class Cancellation : ICancellation {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Cancellation(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override CancellationToken Token =>
        (_httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext)))
        .RequestAborted;
}