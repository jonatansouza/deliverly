using DeliverlyCore.Shared.Domain;
using System.Globalization;

namespace DeliverlyCore.Pricing.Domain.ObjectValue
{
    public sealed class Money : ValueObject<Money>
    {
        // business rule [Precision]: decimal avoids binary floating-point inaccuracies
        public decimal Amount { get; }

        // business rule [Currency Matching]: stored as ISO 4217 code
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        // task [Create]: validates against negative values
        public static Result<Money> Create(decimal amount, string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return Result<Money>.Failure("Currency cannot be empty.");

            if (amount < 0)
                return Result<Money>.Failure("Amount must not be negative.");

            return Result<Money>.Success(new Money(amount, currency.ToUpperInvariant()));
        }

        // task [Arithmetic]: Add — fails if currencies differ
        public Result<Money> Add(Money other)
        {
            if (Currency != other.Currency)
                return Result<Money>.Failure($"Cannot add currencies '{Currency}' and '{other.Currency}'.");

            return Result<Money>.Success(new Money(Amount + other.Amount, Currency));
        }

        // task [Arithmetic]: Subtract — fails if currencies differ or result is negative
        public Result<Money> Subtract(Money other)
        {
            if (Currency != other.Currency)
                return Result<Money>.Failure($"Cannot subtract currencies '{Currency}' and '{other.Currency}'.");

            var result = Amount - other.Amount;
            if (result < 0)
                return Result<Money>.Failure("Subtraction result must not be negative.");

            return Result<Money>.Success(new Money(result, Currency));
        }

        // task [Multiply]: applies a factor (e.g., fuel surcharges)
        public Money Multiply(decimal factor) => new(Amount * factor, Currency);

        // task [Formatting]: human-readable format e.g. "R$ 10,00"
        public string ToFormattedString()
        {
            var culture = Currency switch
            {
                "BRL" => new CultureInfo("pt-BR"),
                "USD" => new CultureInfo("en-US"),
                "EUR" => new CultureInfo("de-DE"),
                _     => CultureInfo.InvariantCulture
            };
            return Amount.ToString("C", culture);
        }

        // business rule [Equality]: same Amount AND Currency
        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }
}
