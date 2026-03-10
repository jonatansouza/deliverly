using DeliverlyCore.Pricing.Domain.ObjectValue;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.Entities
{
    public class TariffTable : Entity
    {
        public string Description { get; private init; }

        // business rule [Data Integrity]: prefix integrity delegated to ZipCode VO
        public ZipCode OriginPrefix { get; private init; }
        public ZipCode DestinationPrefix { get; private init; }

        // business rule [Weight Brackets]: cargo weight must fall within this range
        public Weight MinWeight { get; private init; }
        public Weight MaxWeight { get; private init; }

        // business rule [Currency Consistency]: financial value via Money VO
        public Money BaseValue { get; private init; }

        private TariffTable(
            string description,
            ZipCode originPrefix,
            ZipCode destinationPrefix,
            Weight minWeight,
            Weight maxWeight,
            Money baseValue)
        {
            Description = description;
            OriginPrefix = originPrefix;
            DestinationPrefix = destinationPrefix;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            BaseValue = baseValue;
        }

        // task [Validation]: ZipCode VO ensures no hyphens/spaces in prefixes;
        //                    MinWeight must be strictly less than MaxWeight
        public static Result<TariffTable> Create(
            string description,
            ZipCode originPrefix,
            ZipCode destinationPrefix,
            Weight minWeight,
            Weight maxWeight,
            Money baseValue)
        {
            if (minWeight.Value >= maxWeight.Value)
                return Result<TariffTable>.Failure("MinWeight must be strictly less than MaxWeight.");

            return Result<TariffTable>.Success(new TariffTable(
                description,
                originPrefix,
                destinationPrefix,
                minWeight,
                maxWeight,
                baseValue));
        }

        // task [Eligibility]: checks ZipCode prefix match and Weight within range
        public bool IsMatch(ZipCode originZip, ZipCode destinationZip, Weight weight) =>
            originZip.Value.StartsWith(OriginPrefix.Value)
            && destinationZip.Value.StartsWith(DestinationPrefix.Value)
            && weight.Value >= MinWeight.Value
            && weight.Value <= MaxWeight.Value;

        // task [SpecificityScore]: combined prefix length used to rank rules (Longest Prefix Match)
        public int SpecificityScore => OriginPrefix.Value.Length + DestinationPrefix.Value.Length;
    }
}
