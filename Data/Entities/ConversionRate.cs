using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Share.Contracts;
using Share.ConversionRates.Constraints;
namespace Data.Entities;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class ConversionRate : HasFullAudit {
    [MinLength(CurrencyConstraints.MinLength)]
    [MaxLength(CurrencyConstraints.MaxLength)]
    public string FromCurrency { get; set; } = null!;

    [MinLength(CurrencyConstraints.MinLength)]
    [MaxLength(CurrencyConstraints.MaxLength)]
    public string ToCurrency { get; set; } = null!;

    [Range(CurrencyConstraints.MinAmount, double.MaxValue)]

    public double Amount { get; set; }
}