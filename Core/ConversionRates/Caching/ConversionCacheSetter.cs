using Core.Enums;
using Data.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Scrutor.AspNetCore;
using Share.Contracts;
using Share.DbContracts;
using Share.Helpers;

namespace Core.ConversionRates.Caching;

[UsedImplicitly]
public class ConversionCacheSetter : ICaching<Dictionary<string, Dictionary<string, double>>>, IScopedLifetime {
    private readonly CachingService _cachingService;
    private readonly IReadOnlyRepo<ConversionRate> _conversionRepo;

    public ConversionCacheSetter(CachingService cachingService, IReadOnlyRepo<ConversionRate> conversionRepo)
    {
        _cachingService = cachingService;
        _conversionRepo = conversionRepo;
    }

    public async Task<Dictionary<string, Dictionary<string, double>>> GetOrSet(CancellationToken cancellationToken)
    {
        

        var conversionRates = await _conversionRepo
            .Query()
            .ToListAsync(cancellationToken);

        var graph = GenerateConversionInGraph(conversionRates);

        return await _cachingService.GetOrSet(CacheKeys.ConversionRates, graph);
    }

    public async Task<Dictionary<string, Dictionary<string, double>>> Reset(CancellationToken cancellationToken)
    {
        var conversionRates = await _conversionRepo
            .Query()
            .ToListAsync(cancellationToken);

        var graph = GenerateConversionInGraph(conversionRates);
        
        return await _cachingService.Reset(CacheKeys.ConversionRates, graph);
    }

    private Dictionary<string, Dictionary<string, double>> GenerateConversionInGraph(
        IEnumerable<ConversionRate> conversionRates)
    {
        var graph = new Dictionary<string, Dictionary<string, double>>();
        foreach (var item in conversionRates)
        {
            if (!graph.ContainsKey(item.FromCurrency))
                graph[item.FromCurrency] = new Dictionary<string, double>();

            graph[item.FromCurrency][item.ToCurrency] = item.Amount;

            if (!graph.ContainsKey(item.ToCurrency))
                graph[item.ToCurrency] = new Dictionary<string, double>();

            graph[item.ToCurrency][item.FromCurrency] = 1 / item.Amount;
        }

        return graph;
    }
}