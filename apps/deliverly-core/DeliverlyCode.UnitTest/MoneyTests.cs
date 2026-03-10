using DeliverlyCore.Pricing.Domain.ValueObjects;
using System.Globalization;

namespace DeliverlyCode.UnitTest
{
    [TestFixture]
    public class MoneyTests
    {
        // task [Create]
        [Test]
        public void Create_WithValidAmountAndCurrency_ShouldSucceed()
        {
            var result = Money.Create(10.50m, "BRL");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Amount, Is.EqualTo(10.50m));
            Assert.That(result.Value.Currency, Is.EqualTo("BRL"));
        }

        [Test]
        public void Create_ShouldNormalizeCurrencyToUpperCase()
        {
            var result = Money.Create(10m, "brl");

            Assert.That(result.Value.Currency, Is.EqualTo("BRL"));
        }

        [Test]
        public void Create_WithZeroAmount_ShouldSucceed()
        {
            var result = Money.Create(0m, "BRL");

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Create_WithNegativeAmount_ShouldReturnFailure()
        {
            var result = Money.Create(-1m, "BRL");

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithEmptyCurrency_ShouldReturnFailure()
        {
            var result = Money.Create(10m, string.Empty);

            Assert.That(result.IsFailure, Is.True);
        }

        // business rule [Precision]
        [Test]
        public void Amount_ShouldBeDecimal()
        {
            var property = typeof(Money).GetProperty(nameof(Money.Amount));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(decimal)));
        }

        // business rule [Immutability]
        [Test]
        public void Amount_ShouldHaveNoPublicSetter()
        {
            var property = typeof(Money).GetProperty(nameof(Money.Amount));
            Assert.That(property!.CanWrite, Is.False);
        }

        [Test]
        public void Currency_ShouldHaveNoPublicSetter()
        {
            var property = typeof(Money).GetProperty(nameof(Money.Currency));
            Assert.That(property!.CanWrite, Is.False);
        }

        // task [Arithmetic] - Add
        [Test]
        public void Add_WithSameCurrency_ShouldReturnSum()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(5m, "BRL").Value;

            var result = a.Add(b);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Amount, Is.EqualTo(15m));
        }

        [Test]
        public void Add_WithDifferentCurrency_ShouldReturnFailure()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(5m, "USD").Value;

            var result = a.Add(b);

            Assert.That(result.IsFailure, Is.True);
        }

        // task [Arithmetic] - Subtract
        [Test]
        public void Subtract_WithSameCurrency_ShouldReturnDifference()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(3m, "BRL").Value;

            var result = a.Subtract(b);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Amount, Is.EqualTo(7m));
        }

        [Test]
        public void Subtract_WithDifferentCurrency_ShouldReturnFailure()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(3m, "USD").Value;

            var result = a.Subtract(b);

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Subtract_WhenResultIsNegative_ShouldReturnFailure()
        {
            var a = Money.Create(3m, "BRL").Value;
            var b = Money.Create(10m, "BRL").Value;

            var result = a.Subtract(b);

            Assert.That(result.IsFailure, Is.True);
        }

        // task [Multiply]
        [Test]
        public void Multiply_ShouldReturnScaledAmount()
        {
            var money = Money.Create(100m, "BRL").Value;

            var result = money.Multiply(1.10m);

            Assert.That(result.Amount, Is.EqualTo(110m));
            Assert.That(result.Currency, Is.EqualTo("BRL"));
        }

        [Test]
        public void Multiply_ByZero_ShouldReturnZeroAmount()
        {
            var money = Money.Create(100m, "BRL").Value;

            var result = money.Multiply(0m);

            Assert.That(result.Amount, Is.EqualTo(0m));
        }

        // task [Formatting]
        [Test]
        public void ToFormattedString_BRL_ShouldReturnBrazilianFormat()
        {
            var money = Money.Create(10m, "BRL").Value;
            var expected = 10m.ToString("C", new CultureInfo("pt-BR"));

            Assert.That(money.ToFormattedString(), Is.EqualTo(expected));
        }

        [Test]
        public void ToFormattedString_USD_ShouldReturnUSDollarFormat()
        {
            var money = Money.Create(10m, "USD").Value;
            var expected = 10m.ToString("C", new CultureInfo("en-US"));

            Assert.That(money.ToFormattedString(), Is.EqualTo(expected));
        }

        // business rule [Equality]
        [Test]
        public void TwoMoneys_WithSameAmountAndCurrency_ShouldBeEqual()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(10m, "BRL").Value;

            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void TwoMoneys_WithDifferentCurrency_ShouldNotBeEqual()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(10m, "USD").Value;

            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void TwoMoneys_WithDifferentAmount_ShouldNotBeEqual()
        {
            var a = Money.Create(10m, "BRL").Value;
            var b = Money.Create(20m, "BRL").Value;

            Assert.That(a, Is.Not.EqualTo(b));
        }
    }
}
