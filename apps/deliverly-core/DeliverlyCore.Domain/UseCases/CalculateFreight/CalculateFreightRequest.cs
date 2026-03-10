namespace DeliverlyCore.Pricing.Domain.UseCases.CalculateFreight
{
    public record CalculateFreightRequest(
        string OriginZip,
        string DestinationZip,
        decimal WeightKg);
}
