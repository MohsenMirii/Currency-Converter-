#region

using Core.Enums;
using Data.Entities;
using JetBrains.Annotations;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Scrutor.AspNetCore;
using Share.Contracts;
using Share.DbContracts;
using Share.Helpers;

#endregion

namespace Core.ConversionRates.Caching;

[UsedImplicitly]
public class ConversionCacheSetter : ICaching<List<ConversionCacheDto>>, IScopedLifetime {
    private readonly CachingService _cachingService;
    private readonly IReadOnlyRepo<ConversionRate> _conversionRepo;

    public ConversionCacheSetter(CachingService cachingService, IReadOnlyRepo<ConversionRate> conversionRepo)
    {
        _cachingService = cachingService;
        _conversionRepo = conversionRepo;
    }

    public Task<List<ConversionCacheDto>> GetOrSet(CancellationToken cancellationToken)
    {
        return _cachingService.GetOrSet(CacheKeys.ConversionRates, () => _conversionRepo
            .Query()
            .ProjectToType<ConversionCacheDto>()
            .ToListAsync(cancellationToken));
    }
}