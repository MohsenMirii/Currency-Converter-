using JetBrains.Annotations;
using MediatR;

namespace Share.ConversionRates.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ClearConfigurationCommand : IRequest<Unit> {
}