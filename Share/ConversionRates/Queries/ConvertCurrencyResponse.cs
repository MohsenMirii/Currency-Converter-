using JetBrains.Annotations;

namespace Share.ConversionRates.Queries;

[PublicAPI]
public class ConvertCurrencyResponse {
    public double Result { get; set; }
}