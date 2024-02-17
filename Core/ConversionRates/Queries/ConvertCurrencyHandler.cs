using JetBrains.Annotations;
using MediatR;
using Share.Contracts;
using Share.ConversionRates.Queries;
using Share.Exceptions;

namespace Core.ConversionRates.Queries;

[UsedImplicitly]
public class ConvertCurrencyHandler : IRequestHandler<ConvertCurrencyQuery, ConvertCurrencyResponse> {
    private readonly ICaching<Dictionary<string, Dictionary<string, double>>> _conversionCache;

    public ConvertCurrencyHandler(ICaching<Dictionary<string, Dictionary<string, double>>> conversionCache)
    {
        _conversionCache = conversionCache;
    }

    public async Task<ConvertCurrencyResponse> Handle(ConvertCurrencyQuery request,
        CancellationToken cancellationToken)
    {
        var conversionRates = await _conversionCache.GetOrSet(cancellationToken);

        var convertedRate = Convert(
            request.FromCurrency,
            request.ToCurrency,
            request.Amount,
            conversionRates
        );
        
        return new ConvertCurrencyResponse
        {
            Result = convertedRate
        };
    }

    private static double Convert(string sourceCurrency, string targetCurrency, double amount,Dictionary<string, Dictionary<string, double>> graph)
    {
        if (!graph.ContainsKey(sourceCurrency) || !graph.ContainsKey(targetCurrency))
            throw new NotFound404Exception("Desired data not found in the data center");

        var distances = new Dictionary<string, double>();
        var visited = new HashSet<string>();

        foreach (var currency in graph.Keys)
            distances[currency] = double.MaxValue;

        distances[sourceCurrency] = 1;

        while (true)
        {
            string minCurrency = null;
            foreach (var currency in graph.Keys)
            {
                if (!visited.Contains(currency) && (minCurrency == null || distances[currency] < distances[minCurrency]))
                {
                    minCurrency = currency;
                }
            }

            if (minCurrency == null)
                break;

            visited.Add(minCurrency);

            foreach (var neighbor in graph[minCurrency])
            {
                distances[neighbor.Key] = Math.Min(distances[neighbor.Key], distances[minCurrency] * neighbor.Value);
            }
        }

        return amount * distances[targetCurrency];
    }
}