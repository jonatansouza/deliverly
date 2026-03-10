using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public class DeleteTariffTableUseCase : IUseCase<Guid, bool>
    {
        private readonly ITariffRepository _repository;

        public DeleteTariffTableUseCase(ITariffRepository repository) => _repository = repository;

        public async Task<Result<bool>> ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _repository.GetByIdAsync(id, ct);
            if (existing is null)
                return Result<bool>.Failure($"TariffTable '{id}' not found.");

            await _repository.DeleteAsync(id, ct);
            return Result<bool>.Success(true);
        }
    }
}
