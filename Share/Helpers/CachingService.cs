using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Scrutor.AspNetCore;


namespace Share.Helpers;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class CachingService : ISelfScopedLifetime {
    private readonly IMemoryCache _memoryCache;

    public CachingService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrSet<T>(string key, T setterFunction)
    {
#pragma warning disable CS8600
        if (_memoryCache.TryGetValue(key, out T cachedValue))
#pragma warning restore CS8600

            return cachedValue!;

        cachedValue =  setterFunction;
        _memoryCache.Set(key, cachedValue);

        return cachedValue;
    }

    public async Task<T> Reset<T>(string key, T setterFunction)
    {
        var cachedValue =  setterFunction;
        _memoryCache.Set(key, cachedValue);

        return cachedValue;
    }
}