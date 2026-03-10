using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class GetTariffTableByIdUseCase : IUseCase<Guid, TariffTableResponse>
    {
        private readonly ITariffRepository _repository;

        public GetTariffTableByIdUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<TariffTableResponse>> ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var tariff = await _repository.GetByIdAsync(id, ct);

            if (tariff is null)
                return Result<TariffTableResponse>.Failure($"TariffTable '{id}' not found.");

            return Result<TariffTableResponse>.Success(CreateTariffTableUseCase.ToResponse(tariff));
        }
    }
}
