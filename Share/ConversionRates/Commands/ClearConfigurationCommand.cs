#region

using JetBrains.Annotations;
using MediatR;

#endregion

namespace Share.ConversionRates.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ClearConfigurationCommand : IRequest<Unit> {
}