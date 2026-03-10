using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ObjectValue;

namespace DeliverlyCode.UnitTest
{
    [TestFixture]
    public class TariffTableTests
    {
        private static Money DefaultBaseValue() => Money.Create(50m, "BRL").Value;

        private static TariffTable CreateValid(
            string originPrefix = "01",
            string destinationPrefix = "02",
            decimal minWeight = 0m,
            decimal maxWeight = 100m) =>
            TariffTable.Create("Test Rule", originPrefix, destinationPrefix, minWeight, maxWeight, DefaultBaseValue()).Value;

        // task [Validation] -------------------------------------------------------

        [Test]
        public void Create_WithValidPrefixes_ShouldSucceed()
        {
            var result = TariffTable.Create("Rule", "011", "021", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Create_WithOriginPrefixContainingHyphen_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "01-1", "021", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithDestinationPrefixContainingHyphen_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "011", "02-1", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithOriginPrefixContainingSpace_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "01 1", "021", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithDestinationPrefixContainingSpace_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "011", "02 1", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithEmptyOriginPrefix_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", string.Empty, "021", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithEmptyDestinationPrefix_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "011", string.Empty, 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithOriginPrefixExceedingFiveDigits_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "012345", "021", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithMaxLengthPrefixes_ShouldSucceed()
        {
            var result = TariffTable.Create("Rule", "01234", "56789", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Create_WithSingleDigitPrefixes_ShouldSucceed()
        {
            var result = TariffTable.Create("Rule", "0", "1", 0m, 50m, DefaultBaseValue());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void Create_WithMaxWeightNotGreaterThanMinWeight_ShouldReturnFailure()
        {
            var result = TariffTable.Create("Rule", "01", "02", 50m, 50m, DefaultBaseValue());

            Assert.That(result.IsFailure, Is.True);
        }

        // business rule [Data Integrity] ------------------------------------------

        [Test]
        public void OriginPrefix_ShouldContainOnlyDigits()
        {
            var tariff = CreateValid(originPrefix: "01234");

            Assert.That(tariff.OriginPrefix.All(char.IsDigit), Is.True);
        }

        [Test]
        public void DestinationPrefix_ShouldContainOnlyDigits()
        {
            var tariff = CreateValid(destinationPrefix: "56789");

            Assert.That(tariff.DestinationPrefix.All(char.IsDigit), Is.True);
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
            var tariff = CreateValid(originPrefix: "01", destinationPrefix: "02", minWeight: 0m, maxWeight: 100m);

            Assert.That(tariff.IsMatch("01310-100", "02010-000", 50m), Is.True);
        }

        [Test]
        public void IsMatch_WhenOriginZipDoesNotStartWithPrefix_ShouldReturnFalse()
        {
            var tariff = CreateValid(originPrefix: "01");

            Assert.That(tariff.IsMatch("99999-000", "02010-000", 50m), Is.False);
        }

        [Test]
        public void IsMatch_WhenDestinationZipDoesNotStartWithPrefix_ShouldReturnFalse()
        {
            var tariff = CreateValid(destinationPrefix: "02");

            Assert.That(tariff.IsMatch("01310-100", "99999-000", 50m), Is.False);
        }

        [Test]
        public void IsMatch_WhenWeightIsBelowMinWeight_ShouldReturnFalse()
        {
            var tariff = CreateValid(minWeight: 10m, maxWeight: 100m);

            Assert.That(tariff.IsMatch("01310-100", "02010-000", 5m), Is.False);
        }

        [Test]
        public void IsMatch_WhenWeightIsAboveMaxWeight_ShouldReturnFalse()
        {
            var tariff = CreateValid(minWeight: 0m, maxWeight: 100m);

            Assert.That(tariff.IsMatch("01310-100", "02010-000", 150m), Is.False);
        }

        // business rule [Weight Brackets]: boundary values are inclusive
        [Test]
        public void IsMatch_WhenWeightEqualsMinWeight_ShouldReturnTrue()
        {
            var tariff = CreateValid(minWeight: 10m, maxWeight: 100m);

            Assert.That(tariff.IsMatch("01310-100", "02010-000", 10m), Is.True);
        }

        [Test]
        public void IsMatch_WhenWeightEqualsMaxWeight_ShouldReturnTrue()
        {
            var tariff = CreateValid(minWeight: 0m, maxWeight: 100m);

            Assert.That(tariff.IsMatch("01310-100", "02010-000", 100m), Is.True);
        }

        // task [SpecificityScore] -------------------------------------------------

        [Test]
        public void SpecificityScore_ShouldReturnCombinedPrefixLength()
        {
            var tariff = CreateValid(originPrefix: "011", destinationPrefix: "02");

            Assert.That(tariff.SpecificityScore, Is.EqualTo(5));
        }

        [Test]
        public void SpecificityScore_WithMaxLengthPrefixes_ShouldReturnTen()
        {
            var tariff = CreateValid(originPrefix: "01234", destinationPrefix: "56789");

            Assert.That(tariff.SpecificityScore, Is.EqualTo(10));
        }

        // business rule [Longest Prefix Match] ------------------------------------

        [Test]
        public void SpecificityScore_MoreSpecificRule_ShouldOutrankBroaderRule()
        {
            var broadRule = CreateValid(originPrefix: "0", destinationPrefix: "0");
            var specificRule = CreateValid(originPrefix: "011", destinationPrefix: "021");

            Assert.That(specificRule.SpecificityScore, Is.GreaterThan(broadRule.SpecificityScore));
        }
    }
}
