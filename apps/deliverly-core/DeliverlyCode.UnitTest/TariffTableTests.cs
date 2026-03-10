using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ValueObjects;

namespace DeliverlyCode.UnitTest
{
    [TestFixture]
    public class TariffTableTests
    {
        private static Money DefaultBaseValue() => Money.Create(50m, "BRL").Value;
        private static ZipCode Zip(string raw) => ZipCode.Create(raw).Value;
        private static Weight Kg(decimal v) => Weight.Create(v).Value;

        private static TariffTable CreateValid(
            string originPrefix = "01310100",
            string destinationPrefix = "02010000",
            decimal minKg = 1m,
            decimal maxKg = 100m) =>
            TariffTable.Create(
                "Test Rule",
                Zip(originPrefix),
                Zip(destinationPrefix),
                Kg(minKg),
                Kg(maxKg),
                DefaultBaseValue()).Value;

        // task [Validation] -------------------------------------------------------

        [Test]
        public void Create_WithValidInputs_ShouldSucceed()
        {
            var result = TariffTable.Create("Rule", Zip("01310100"), Zip("02010000"), Kg(1m), Kg(50m), DefaultBaseValue());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Create_WithMinWeightEqualToMaxWeight_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", Zip("01310100"), Zip("02010000"), Kg(50m), Kg(50m), DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithMinWeightGreaterThanMaxWeight_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", Zip("01310100"), Zip("02010000"), Kg(100m), Kg(10m), DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithOriginPrefixContainingHyphen_ShouldReturnFailureFromZipCode()
        {
            var result = ZipCode.Create("0131-0100");

            // ZipCode VO strips hyphens and validates 8 numeric digits
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Value, Is.EqualTo("01310100"));
        }

        [Test]
        public void Create_WithOriginPrefixContainingLetters_ShouldFailAtZipCodeLevel()
        {
            var result = ZipCode.Create("0131ABCD");

            Assert.That(result.IsFailure, Is.True);
        }

        // business rule [Data Integrity] ------------------------------------------

        [Test]
        public void OriginPrefix_ShouldBeZipCodeValueObject()
        {
            var property = typeof(TariffTable).GetProperty(nameof(TariffTable.OriginPrefix));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(ZipCode)));
        }

        [Test]
        public void DestinationPrefix_ShouldBeZipCodeValueObject()
        {
            var property = typeof(TariffTable).GetProperty(nameof(TariffTable.DestinationPrefix));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(ZipCode)));
        }

        [Test]
        public void OriginPrefix_ShouldContainOnlyDigits()
        {
            var tariff = CreateValid();

            Assert.That(tariff.OriginPrefix.Value.All(char.IsDigit), Is.True);
        }

        // business rule [Weight Brackets] -----------------------------------------

        [Test]
        public void MinWeight_ShouldBeWeightValueObject()
        {
            var property = typeof(TariffTable).GetProperty(nameof(TariffTable.MinWeight));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(Weight)));
        }

        [Test]
        public void MaxWeight_ShouldBeWeightValueObject()
        {
            var property = typeof(TariffTable).GetProperty(nameof(TariffTable.MaxWeight));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(Weight)));
        }

        // business rule [Currency Consistency] ------------------------------------

        [Test]
        public void BaseValue_ShouldBeMoneyValueObject()
        {
            var property = typeof(TariffTable).GetProperty(nameof(TariffTable.BaseValue));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(Money)));
        }

        // task [Eligibility] ------------------------------------------------------

        [Test]
        public void IsMatch_WhenZipsAndWeightMatch_ShouldReturnTrue()
        {
            var tariff = CreateValid(originPrefix: "01310100", destinationPrefix: "02010000", minKg: 1m, maxKg: 100m);

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("02010000"), Kg(50m)), Is.True);
        }

        [Test]
        public void IsMatch_WhenOriginZipDoesNotStartWithPrefix_ShouldReturnFalse()
        {
            var tariff = CreateValid(originPrefix: "01310100");

            Assert.That(tariff.IsMatch(Zip("99999000"), Zip("02010000"), Kg(50m)), Is.False);
        }

        [Test]
        public void IsMatch_WhenDestinationZipDoesNotStartWithPrefix_ShouldReturnFalse()
        {
            var tariff = CreateValid(destinationPrefix: "02010000");

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("99999000"), Kg(50m)), Is.False);
        }

        [Test]
        public void IsMatch_WhenWeightIsBelowMinWeight_ShouldReturnFalse()
        {
            var tariff = CreateValid(minKg: 10m, maxKg: 100m);

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("02010000"), Kg(5m)), Is.False);
        }

        [Test]
        public void IsMatch_WhenWeightIsAboveMaxWeight_ShouldReturnFalse()
        {
            var tariff = CreateValid(minKg: 1m, maxKg: 50m);

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("02010000"), Kg(100m)), Is.False);
        }

        [Test]
        public void IsMatch_WhenWeightEqualsMinWeight_ShouldReturnTrue()
        {
            var tariff = CreateValid(minKg: 10m, maxKg: 100m);

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("02010000"), Kg(10m)), Is.True);
        }

        [Test]
        public void IsMatch_WhenWeightEqualsMaxWeight_ShouldReturnTrue()
        {
            var tariff = CreateValid(minKg: 1m, maxKg: 100m);

            Assert.That(tariff.IsMatch(Zip("01310100"), Zip("02010000"), Kg(100m)), Is.True);
        }

        // task [SpecificityScore] -------------------------------------------------

        [Test]
        public void SpecificityScore_ShouldReturnCombinedPrefixValueLength()
        {
            var tariff = CreateValid(originPrefix: "01310100", destinationPrefix: "02010000");

            Assert.That(tariff.SpecificityScore, Is.EqualTo(16));
        }

        // business rule [Longest Prefix Match] ------------------------------------

        [Test]
        public void SpecificityScore_ShouldEqualSumOfOriginAndDestinationPrefixLengths()
        {
            var tariff = CreateValid();

            Assert.That(tariff.SpecificityScore,
                Is.EqualTo(tariff.OriginPrefix.Value.Length + tariff.DestinationPrefix.Value.Length));
        }
    }
}
