using DeliverlyCore.Pricing.Domain.ObjectValue;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.Entities
{
    public class TariffTable : Entity
    {
        public string Description { get; private init; }

        // business rule [Data Integrity]: only digits, 1–5 chars
        public string OriginPrefix { get; private init; }
        public string DestinationPrefix { get; private init; }

        // business rule [Weight Brackets]: cargo weight must fall within this range
        public decimal MinWeight { get; private init; }
        public decimal MaxWeight { get; private init; }

        // business rule [Currency Consistency]: financial value via Money VO
        public Money BaseValue { get; private init; }

        private TariffTable(
            string description,
            string originPrefix,
            string destinationPrefix,
            decimal minWeight,
            decimal maxWeight,
            Money baseValue)
        {
            Description = description;
            OriginPrefix = originPrefix;
            DestinationPrefix = destinationPrefix;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            BaseValue = baseValue;
        }

        // task [Validation]: prefixes must contain only digits (no hyphens or spaces), length 1–5
        public static Result<TariffTable> Create(
            string description,
            string originPrefix,
            string destinationPrefix,
            decimal minWeight,
            decimal maxWeight,
            Money baseValue)
        {
            var originError = ValidatePrefix(originPrefix, nameof(originPrefix));
            if (originError is not null)
                return Result<TariffTable>.Failure(originError);

            var destinationError = ValidatePrefix(destinationPrefix, nameof(destinationPrefix));
            if (destinationError is not null)
                return Result<TariffTable>.Failure(destinationError);

            if (maxWeight <= minWeight)
                return Result<TariffTable>.Failure("MaxWeight must be greater than MinWeight.");

            return Result<TariffTable>.Success(new TariffTable(
                description,
                originPrefix,
                destinationPrefix,
                minWeight,
                maxWeight,
                baseValue));
        }

        private static string? ValidatePrefix(string prefix, string paramName)
        {
            if (string.IsNullOrEmpty(prefix))
                return $"{paramName} must not be empty.";

            if (prefix.Length > 5)
                return $"{paramName} must be at most 5 digits.";

            if (!prefix.All(char.IsDigit))
                return $"{paramName} must contain only digits (no hyphens or spaces).";

            return null;
        }

        // task [Eligibility]: checks zip prefix match and weight within range
        public bool IsMatch(string originZip, string destinationZip, decimal weight) =>
            originZip.StartsWith(OriginPrefix)
            && destinationZip.StartsWith(DestinationPrefix)
            && weight >= MinWeight
            && weight <= MaxWeight;

        // task [SpecificityScore]: combined prefix length used to rank rules (Longest Prefix Match)
        public int SpecificityScore => OriginPrefix.Length + DestinationPrefix.Length;
    }
}
