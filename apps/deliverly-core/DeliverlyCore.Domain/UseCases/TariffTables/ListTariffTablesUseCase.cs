using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class ListTariffTablesUseCase : IUseCase<object?, IReadOnlyList<TariffTableResponse>>
    {
        private readonly ITariffRepository _repository;

        public ListTariffTablesUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<IReadOnlyList<TariffTableResponse>>> ExecuteAsync(object? input, CancellationToken ct = default)
        {
            var tariffs = await _repository.GetAllAsync(ct);
            var response = tariffs.Select(CreateTariffTableUseCase.ToResponse).ToList();
            return Result<IReadOnlyList<TariffTableResponse>>.Success(response);
        }
    }
}
