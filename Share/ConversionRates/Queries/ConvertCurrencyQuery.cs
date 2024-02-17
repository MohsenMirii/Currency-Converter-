using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using MediatR;
using Share.ConversionRates.Constraints;
using Share.Helpers;

namespace Share.ConversionRates.Queries;

[PublicAPI]
public class ConvertCurrencyQuery : IRequest<ConvertCurrencyResponse> {
    private string _fromCurrency;
    private string _toCurrency;

    [Required(ErrorMessage = "FromCurrency is required")]
    public string FromCurrency
    {
        get => _fromCurrency;
        set => _fromCurrency = value.Clean();
    }

    [Required(ErrorMessage = "ToCurrency is required")]
    public string ToCurrency
    {
        get => _toCurrency;
        set => _toCurrency = value.Clean();
    }

    [Required(ErrorMessage = "Amount is required")]
    [Range(CurrencyConstraints.MinAmount, double.MaxValue, ErrorMessage = "Amount must be a positive number")]
    public double Amount { get; set; }
}