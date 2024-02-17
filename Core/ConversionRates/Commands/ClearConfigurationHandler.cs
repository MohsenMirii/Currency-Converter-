using Data.Entities;
using JetBrains.Annotations;
using MediatR;
using Share.Contracts;
using Share.ConversionRates.Commands;
using Share.DbContracts;

namespace Core.ConversionRates.Commands;

[UsedImplicitly]
public class ClearConfigurationHandler : IRequestHandler<ClearConfigurationCommand, Unit> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepo<ConversionRate> _conversionRepo;
    private readonly ICaching<Dictionary<string, Dictionary<string, double>>> _conversionCache;

    public ClearConfigurationHandler(IRepo<ConversionRate> conversionRepo,
        IUnitOfWork unitOfWork,
        ICaching<Dictionary<string, Dictionary<string, double>>> conversionCache)
    {
        _conversionRepo = conversionRepo;
        _unitOfWork = unitOfWork;
        _conversionCache = conversionCache;
    }

    public async Task<Unit> Handle(ClearConfigurationCommand request, CancellationToken cancellationToken)
    {
        // soft delete all previous data 
        _conversionRepo.Delete(_ => true);

        // save the soft delete
        await _unitOfWork.SaveChangesAsync();

        // clean cache
        await _conversionCache.Reset(cancellationToken);

        return Unit.Value;
    }
    
    
}