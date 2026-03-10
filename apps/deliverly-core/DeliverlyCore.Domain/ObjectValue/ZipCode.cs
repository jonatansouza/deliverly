using DeliverlyCore.Shared.Domain;
using System.Text.RegularExpressions;

namespace DeliverlyCore.Pricing.Domain.ObjectValue
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

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
