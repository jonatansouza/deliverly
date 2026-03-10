using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ValueObjects;

namespace DeliverlyCore.Pricing.Domain.Ports
{
    public interface ITariffRepository
    {
        Task<TariffTable?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<TariffTable>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(TariffTable tariff, CancellationToken ct = default);
        Task UpdateAsync(TariffTable tariff, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<IReadOnlyList<TariffTable>> FindPossibleMatchesAsync(
            IEnumerable<string> originPrefixes,
            IEnumerable<string> destPrefixes,
            Weight weight,
            CancellationToken ct = default);
    }
}
