#region

using JetBrains.Annotations;
using MediatR;
using Share.ConversionRates.Constraints;
using Share.Exceptions;

#endregion

namespace Share.ConversionRates.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class UpdateConfigurationCommand : IRequest<Unit> {
    private IEnumerable<Tuple<string, string, double>> _conversionRates = null!;

    public IEnumerable<Tuple<string, string, double>> ConversionRates
    {
        get => _conversionRates;
        set
        {
            // Check if the value is null
            if (value == null)
                throw new BadRequest400Exception("please enter your data");

            var invalidTuples = value.Where(
                tuple => tuple.Item1.Length is > CurrencyConstraints.MaxLength or < CurrencyConstraints.MinLength
                         || tuple.Item2.Length is > CurrencyConstraints.MaxLength or < CurrencyConstraints.MinLength
                         || tuple.Item3 <= CurrencyConstraints.MinRate
                         || tuple.Item1 == tuple.Item2
            ).ToList();

            if (invalidTuples.Any())

                throw new BadRequest400Exception(
                    $"Error: Currency title length must be " +
                    $"between {CurrencyConstraints.MinLength} and {CurrencyConstraints.MaxLength} , "
                    + $"and the Rate must be bigger than {CurrencyConstraints.MinRate}"
                    + $" Invalid tuples: {string.Join(", ", invalidTuples.Select(tuple => $"({tuple.Item1}, {tuple.Item2})"))}");

            _conversionRates = value;
        }
    }
}