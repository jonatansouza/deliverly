namespace DeliverlyCore;

public class PricingConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime EffectiveDate { get; set; }

    public decimal BasePrice { get; set; }

    public double SurgeMultiplier { get; set; }

    public decimal FinalPrice => BasePrice * (decimal)SurgeMultiplier;

    public string? PricingSource { get; set; }

    public string Status { get; set; } = "Draft";
}