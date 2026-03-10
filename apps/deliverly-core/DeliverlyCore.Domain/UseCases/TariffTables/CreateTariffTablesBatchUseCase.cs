using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.ValueObjects;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class CreateTariffTablesBatchUseCase
    {
        private readonly ITariffRepository _repository;

        public CreateTariffTablesBatchUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<IReadOnlyList<TariffTableResponse>>> ExecuteAsync(
            IEnumerable<CreateTariffTableRequest> requests, CancellationToken ct = default)
        {
            var tariffs = new List<TariffTable>();

            foreach (var input in requests)
            {
                var originResult = ZipCode.CreateAsPrefix(input.OriginPrefix);
                if (originResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(originResult.Error);

                var destResult = ZipCode.CreateAsPrefix(input.DestinationPrefix);
                if (destResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(destResult.Error);

                var minWeightResult = Weight.Create(input.MinWeightKg);
                if (minWeightResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(minWeightResult.Error);

                var maxWeightResult = Weight.Create(input.MaxWeightKg);
                if (maxWeightResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(maxWeightResult.Error);

                var moneyResult = Money.Create(input.BaseValueAmount, input.Currency);
                if (moneyResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(moneyResult.Error);

                var tariffResult = TariffTable.Create(
                    input.Description,
                    originResult.Value,
                    destResult.Value,
                    minWeightResult.Value,
                    maxWeightResult.Value,
                    moneyResult.Value);

                if (tariffResult.IsFailure) return Result<IReadOnlyList<TariffTableResponse>>.Failure(tariffResult.Error);

                tariffs.Add(tariffResult.Value);
            }

            await _repository.AddRangeAsync(tariffs, ct);

            IReadOnlyList<TariffTableResponse> response = tariffs.Select(CreateTariffTableUseCase.ToResponse).ToList();
            return Result<IReadOnlyList<TariffTableResponse>>.Success(response);
        }
    }
}
