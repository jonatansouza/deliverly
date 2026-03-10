namespace DeliverlyCore.Pricing.Domain.UseCases.CalculateFreight
{
    public record FreightResponse(
        string OriginZip,
        string DestinationZip,
        decimal WeightKg,
        decimal Price,
        string Currency);
}
