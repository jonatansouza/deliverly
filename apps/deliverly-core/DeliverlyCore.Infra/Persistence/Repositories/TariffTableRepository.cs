using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DeliverlyCore.Infra.Persistence.Repositories
{
    public class TariffTableRepository : ITariffRepository
    {
        private readonly DeliverlyDbContext _context;

        public TariffTableRepository(DeliverlyDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TariffTable>> FindPossibleMatchesAsync(
            IEnumerable<string> originPrefixes,
            IEnumerable<string> destPrefixes,
            Weight weight,
            CancellationToken ct = default)
        {
            var origins = originPrefixes.ToList();
            var dests = destPrefixes.ToList();
            var w = weight.Value;

            return await _context.TariffTables
                .Where(t => origins.Contains(t.OriginPrefix.Value)
                         && dests.Contains(t.DestinationPrefix.Value)
                         && t.MinWeight.Value <= w
                         && t.MaxWeight.Value >= w)
                .ToListAsync(ct);
        }
    }
}
