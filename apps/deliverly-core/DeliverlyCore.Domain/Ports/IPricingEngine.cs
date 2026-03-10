using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ObjectValue;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.Ports
{
    public interface IPricingEngine
    {
        Result<Money> CalculateBestPrice(
            ZipCode origin,
            ZipCode destination,
            Weight weight,
            IReadOnlyList<TariffTable> candidates);
    }
}
