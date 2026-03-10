using DeliverlyCore.Shared.Domain;

namespace DeliverlyCore.Pricing.Domain.ValueObjects
{
    public sealed class Weight : ValueObject<Weight>
    {
        // business rule [Precision]: decimal avoids floating-point inaccuracies
        // business rule [Unit Standardization]: value is always in kilograms
        public decimal Value { get; }

        private Weight(decimal value)
        {
            Value = value;
        }

        // task [Create] + task [Validate]: rejects zero or negative mass
        public static Result<Weight> Create(decimal value)
        {
            // business rule [Physical Constraint]: weight must be positive (> 0)
            if (value <= 0)
                return Result<Weight>.Failure("Weight must be greater than zero.");

            return Result<Weight>.Success(new Weight(value));
        }

        // task [ComparisonOperators]: allow seamless comparison with MinWeight/MaxWeight decimals
        public static bool operator >(Weight left, decimal right) => left.Value > right;
        public static bool operator <(Weight left, decimal right) => left.Value < right;
        public static bool operator >=(Weight left, decimal right) => left.Value >= right;
        public static bool operator <=(Weight left, decimal right) => left.Value <= right;

        public static bool operator >(decimal left, Weight right) => left > right.Value;
        public static bool operator <(decimal left, Weight right) => left < right.Value;
        public static bool operator >=(decimal left, Weight right) => left >= right.Value;
        public static bool operator <=(decimal left, Weight right) => left <= right.Value;

        // task [Formatting]: human-readable string with unit
        public override string ToString() => $"{Value} kg";

        // task [Equality]: two Weight objects are equal if their Value is exactly the same
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
