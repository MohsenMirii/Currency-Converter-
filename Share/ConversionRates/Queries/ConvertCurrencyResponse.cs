#region

using JetBrains.Annotations;

#endregion

namespace Share.ConversionRates.Queries;

[PublicAPI]
public class ConvertCurrencyResponse {
    public double Result { get; set; }
}