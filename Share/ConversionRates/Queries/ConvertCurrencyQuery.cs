#region

using MediatR;

#endregion

namespace Share.ConversionRates.Queries;

public class ConvertCurrencyQuery : IRequest<ConvertCurrencyResponse> {
    public string FromCurrency { get; set; } = null!;
    public string ToCurrency { get; set; } = null!;
    public double Amount { get; set; }
}