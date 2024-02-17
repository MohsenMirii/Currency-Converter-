#region

using Core.ConversionRates.Caching;
using JetBrains.Annotations;
using MediatR;
using Share.Contracts;
using Share.ConversionRates.Queries;
using Share.Exceptions;

#endregion

namespace Core.ConversionRates.Queries;

[UsedImplicitly]
public class ConvertCurrencyHandler : IRequestHandler<ConvertCurrencyQuery, ConvertCurrencyResponse> {
    private readonly ICaching<List<ConversionCacheDto>> _conversionCache;

    public ConvertCurrencyHandler(ICaching<List<ConversionCacheDto>> conversionCache)
    {
        _conversionCache = conversionCache;
    }

    public async Task<ConvertCurrencyResponse> Handle(ConvertCurrencyQuery request,
        CancellationToken cancellationToken)
    {
        var conversionRates = await _conversionCache.GetOrSet(cancellationToken);
        var convertedRate = Convert(request.FromCurrency, request.ToCurrency, request.Amount, conversionRates);

        return new ConvertCurrencyResponse
        {
            Result = convertedRate
        };
    }

    private static double Convert(string fromCurrency, string toCurrency, double amount, List<ConversionCacheDto> conversionRates)
    {
        if (fromCurrency == toCurrency)
            return amount;

        var conversionRate = conversionRates.FirstOrDefault(_ => _.SourceCurrency == fromCurrency && _.DestinationCurrency == toCurrency);

        if (conversionRate != null)
            return amount * conversionRate.Rate;

        // Find conversion path
        foreach (var pair in conversionRates)
        {
            if (pair.SourceCurrency != fromCurrency) continue;
            var intermediateAmount = amount * pair.Rate;
            var result = Convert(pair.DestinationCurrency, toCurrency, intermediateAmount, conversionRates);
            return result;
        }

        throw new NotFound404Exception("No conversion path found.");
    }
}