namespace DeliverlyCore.Pricing.Domain.UseCases.TariffTables
{
    public record TariffTableResponse(
        Guid Id,
        string Description,
        string OriginPrefix,
        string DestinationPrefix,
        decimal MinWeightKg,
        decimal MaxWeightKg,
        decimal BaseValueAmount,
        string BaseValueCurrency,
        DateTime CreatedAt);
}
