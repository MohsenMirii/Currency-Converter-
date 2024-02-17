using Data.Entities;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.ConversionRates.Commands;
using Share.DbContracts;
using Share.Helpers;

namespace Core.ConversionRates.Commands;

[UsedImplicitly]
public class UpdateConfigurationHandler : IRequestHandler<UpdateConfigurationCommand, Unit> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepo<ConversionRate> _conversionRepo;
    private readonly ICaching<Dictionary<string, Dictionary<string, double>>> _conversionCache;

    public UpdateConfigurationHandler(IUnitOfWork unitOfWork, IRepo<ConversionRate> conversionRepo,
        ICaching<Dictionary<string, Dictionary<string, double>>> conversionCache)
    {
        _unitOfWork = unitOfWork;
        _conversionRepo = conversionRepo;
        _conversionCache = conversionCache;
    }

    public async Task<Unit> Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
    {
        foreach (var item in request.ConversionRates)
        {
            // get the conversion rate if exist in db
            var conversionRate = await _conversionRepo
                .Query(_ => _.FromCurrency == item.Item1
                            && _.ToCurrency == item.Item2)
                .FirstOrDefaultAsync(cancellationToken);

            // update conversion rate
            if (conversionRate != null)
            {
                _conversionRepo.UpdateWhere(_ => _.Id == conversionRate.Id)
                    .Set(_ => _.Amount, item.Item3);
                continue;
            }

            // create new conversion rate in db
            conversionRate = new ConversionRate
            {
                FromCurrency = item.Item1.Clean(),
                ToCurrency = item.Item2.Clean(),
                Amount = item.Item3
            };

            _conversionRepo.InitializeNewEntity(conversionRate);
            _conversionRepo.Create(conversionRate);
        }

        // save changes in db
        await _unitOfWork.SaveChangesAsync();

        // set cache with new data
        await _conversionCache.Reset(cancellationToken);

        return Unit.Value;
    }
}