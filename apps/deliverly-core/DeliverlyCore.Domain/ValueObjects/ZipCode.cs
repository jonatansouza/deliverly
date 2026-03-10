using DeliverlyCore.Shared.Domain;
using System.Text.RegularExpressions;

namespace DeliverlyCore.Pricing.Domain.ValueObjects
{
    public sealed class ZipCode : ValueObject<ZipCode>
    {
        private static readonly Regex DigitsOnly = new(@"^\d{8}$", RegexOptions.Compiled);

        public string Value { get; }

        private ZipCode(string value)
        {
            Value = value;
        }

        // task [Create]: strips dashes, stores 8 digits only
        public static Result<ZipCode> Create(string rawZipCode)
        {
            var result = Validate(rawZipCode);
            if (result.IsFailure) return result;

            var digits = rawZipCode.Replace("-", string.Empty);
            return Result<ZipCode>.Success(new ZipCode(digits));
        }

        // task [Validate]: returns Result.Failure if format is invalid
        public static Result<ZipCode> Validate(string rawZipCode)
        {
            if (string.IsNullOrEmpty(rawZipCode))
                return Result<ZipCode>.Failure("ZipCode cannot be empty or null.");

            var digits = rawZipCode.Replace("-", string.Empty);

            if (!DigitsOnly.IsMatch(digits))
                return Result<ZipCode>.Failure("ZipCode must contain exactly 8 numeric digits.");

            return Result<ZipCode>.Success(null!);
        }

        // task [Formatting]: returns pattern 00000-000
        public string ToFormattedString() => $"{Value[..5]}-{Value[5..]}";

        // task [GetRegion]: returns the first digit
        public string GetRegion() => Value[..1];

        // task [GetSubRegion]: returns the first two digits
        public string GetSubRegion() => Value[..2];

        // task [GetSector]: returns the first five digits
        public string GetSector() => Value[..5];

        // task [Comparison]: same micro-zone (same first 5 digits)
        public bool IsSameSector(ZipCode other) => GetSector() == other.GetSector();

        // task [Comparison]: same macro-region (same first digit)
        public bool IsSameRegion(ZipCode other) => GetRegion() == other.GetRegion();

        // task [GetAllPrefixes]: returns all string prefixes from length 1 to 8
        public IEnumerable<string> GetAllPrefixes()
        {
            for (var i = 1; i <= Value.Length; i++)
                yield return Value[..i];
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
