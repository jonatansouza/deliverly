using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ValueObjects;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class CreateTariffTableUseCase : IUseCase<CreateTariffTableRequest, TariffTableResponse>
    {
        private readonly ITariffRepository _repository;

        public CreateTariffTableUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<TariffTableResponse>> ExecuteAsync(CreateTariffTableRequest input, CancellationToken ct = default)
        {
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

            var tariffResult = TariffTable.Create(
                input.Description,
                originResult.Value,
                destResult.Value,
                minWeightResult.Value,
                maxWeightResult.Value,
                moneyResult.Value);

            if (tariffResult.IsFailure) return Result<TariffTableResponse>.Failure(tariffResult.Error);

            await _repository.AddAsync(tariffResult.Value, ct);

            return Result<TariffTableResponse>.Success(ToResponse(tariffResult.Value));
        }

        internal static TariffTableResponse ToResponse(TariffTable t) => new(
            t.Id,
            t.Description,
            t.OriginPrefix.Value,
            t.DestinationPrefix.Value,
            t.MinWeight.Value,
            t.MaxWeight.Value,
            t.BaseValue.Amount,
            t.BaseValue.Currency,
            t.CreatedAt);
    }
}
