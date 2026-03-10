using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.ValueObjects;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class UpdateTariffTableUseCase : IUseCase<UpdateTariffTableRequest, TariffTableResponse>
    {
        private readonly ITariffRepository _repository;

        public UpdateTariffTableUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<TariffTableResponse>> ExecuteAsync(UpdateTariffTableRequest input, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(input.Id, ct);
            if (existing is null)
                return Result<TariffTableResponse>.Failure($"TariffTable '{input.Id}' not found.");

            var originResult = ZipCode.Create(input.OriginPrefix);
            if (originResult.IsFailure) return Result<TariffTableResponse>.Failure(originResult.Error);

            var destResult = ZipCode.Create(input.DestinationPrefix);
            if (destResult.IsFailure) return Result<TariffTableResponse>.Failure(destResult.Error);

            var minWeightResult = Weight.Create(input.MinWeightKg);
            if (minWeightResult.IsFailure) return Result<TariffTableResponse>.Failure(minWeightResult.Error);

            var maxWeightResult = Weight.Create(input.MaxWeightKg);
            if (maxWeightResult.IsFailure) return Result<TariffTableResponse>.Failure(maxWeightResult.Error);

            var moneyResult = Money.Create(input.BaseValueAmount, input.Currency);
            if (moneyResult.IsFailure) return Result<TariffTableResponse>.Failure(moneyResult.Error);

            var reconstituted = TariffTable.Reconstitute(
                input.Id,
                input.Description,
                originResult.Value,
                destResult.Value,
                minWeightResult.Value,
                maxWeightResult.Value,
                moneyResult.Value);

            if (reconstituted.IsFailure) return Result<TariffTableResponse>.Failure(reconstituted.Error);

            await _repository.UpdateAsync(reconstituted.Value, ct);

            return Result<TariffTableResponse>.Success(CreateTariffTableUseCase.ToResponse(reconstituted.Value));
        }
    }
}
