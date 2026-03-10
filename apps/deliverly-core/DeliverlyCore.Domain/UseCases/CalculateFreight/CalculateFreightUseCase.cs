using DeliverlyCore.Pricing.Domain.ObjectValue;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.CalculateFreight
{
    public class CalculateFreightUseCase : IUseCase<CalculateFreightRequest, FreightResponse>
    {
        private readonly ITariffRepository _tariffRepository;
        private readonly IPricingEngine _pricingEngine;

        public CalculateFreightUseCase(ITariffRepository tariffRepository, IPricingEngine pricingEngine)
        {
            _tariffRepository = tariffRepository;
            _pricingEngine = pricingEngine;
        }

        public async Task<Result<FreightResponse>> ExecuteAsync(CalculateFreightRequest input, CancellationToken ct = default)
        {
            // task [TranslateInputs]
            var originResult = ZipCode.Create(input.OriginZip);
            if (originResult.IsFailure) return Result<FreightResponse>.Failure(originResult.Error);

            var destResult = ZipCode.Create(input.DestinationZip);
            if (destResult.IsFailure) return Result<FreightResponse>.Failure(destResult.Error);

            var weightResult = Weight.Create(input.WeightKg);
            if (weightResult.IsFailure) return Result<FreightResponse>.Failure(weightResult.Error);

            var originZip = originResult.Value;
            var destZip = destResult.Value;
            var weight = weightResult.Value;

            // task [PrepareSearch]
            var originPrefixes = originZip.GetAllPrefixes();
            var destPrefixes = destZip.GetAllPrefixes();

            // task [Fetch]
            var candidates = await _tariffRepository.FindPossibleMatchesAsync(originPrefixes, destPrefixes, weight, ct);

            // task [DelegateCalculation]
            var priceResult = _pricingEngine.CalculateBestPrice(originZip, destZip, weight, candidates);

            // task [HandleFailure]
            if (priceResult.IsFailure) return Result<FreightResponse>.Failure(priceResult.Error);

            // task [Return]
            var response = new FreightResponse(
                originZip.Value,
                destZip.Value,
                weight.Value,
                priceResult.Value.Amount,
                priceResult.Value.Currency);

            return Result<FreightResponse>.Success(response);
        }
    }
}
