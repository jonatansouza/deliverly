using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ValueObjects;

namespace DeliverlyCore.Pricing.Domain.Ports
{
    public interface ITariffRepository
    {
        Task<IReadOnlyList<TariffTable>> FindPossibleMatchesAsync(
            IEnumerable<string> originPrefixes,
            IEnumerable<string> destPrefixes,
            Weight weight,
            CancellationToken ct = default);
    }
}
