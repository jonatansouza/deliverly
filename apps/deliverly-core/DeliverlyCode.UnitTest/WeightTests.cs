using DeliverlyCore.Pricing.Domain.ObjectValue;

namespace DeliverlyCode.UnitTest
{
    [TestFixture]
    public class WeightTests
    {
        // task [Create] ----------------------------------------------------------

        [Test]
        public void Create_WithPositiveValue_ShouldSucceed()
        {
            var result = Weight.Create(5.5m);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Value, Is.EqualTo(5.5m));
        }

        [Test]
        public void Create_WithSmallestPossibleValue_ShouldSucceed()
        {
            var result = Weight.Create(0.01m);

            Assert.That(result.IsSuccess, Is.True);
        }

        // task [Validate] --------------------------------------------------------

        [Test]
        public void Create_WithZeroValue_ShouldReturnFailure()
        {
            var result = Weight.Create(0m);

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithNegativeValue_ShouldReturnFailure()
        {
            var result = Weight.Create(-1m);

            Assert.That(result.IsFailure, Is.True);
        }

        // business rule [Precision] ----------------------------------------------

        [Test]
        public void Value_ShouldBeDecimal()
        {
            var property = typeof(Weight).GetProperty(nameof(Weight.Value));

            Assert.That(property!.PropertyType, Is.EqualTo(typeof(decimal)));
        }

        // business rule [Immutability] -------------------------------------------

        [Test]
        public void Value_ShouldHaveNoPublicSetter()
        {
            var property = typeof(Weight).GetProperty(nameof(Weight.Value));

            Assert.That(property!.CanWrite, Is.False);
        }

        // task [ComparisonOperators] ---------------------------------------------

        [Test]
        public void GreaterThan_WhenValueIsLarger_ShouldReturnTrue()
        {
            var weight = Weight.Create(10m).Value;

            Assert.That(weight > 5m, Is.True);
        }

        [Test]
        public void GreaterThan_WhenValueIsSmaller_ShouldReturnFalse()
        {
            var weight = Weight.Create(3m).Value;

            Assert.That(weight > 5m, Is.False);
        }

        [Test]
        public void LessThan_WhenValueIsSmaller_ShouldReturnTrue()
        {
            var weight = Weight.Create(3m).Value;

            Assert.That(weight < 5m, Is.True);
        }

        [Test]
        public void LessThan_WhenValueIsLarger_ShouldReturnFalse()
        {
            var weight = Weight.Create(10m).Value;

            Assert.That(weight < 5m, Is.False);
        }

        [Test]
        public void GreaterThanOrEqual_WhenValuesAreEqual_ShouldReturnTrue()
        {
            var weight = Weight.Create(5m).Value;

            Assert.That(weight >= 5m, Is.True);
        }

        [Test]
        public void LessThanOrEqual_WhenValuesAreEqual_ShouldReturnTrue()
        {
            var weight = Weight.Create(5m).Value;

            Assert.That(weight <= 5m, Is.True);
        }

        [Test]
        public void DecimalGreaterThan_Weight_ShouldWorkCorrectly()
        {
            var weight = Weight.Create(3m).Value;

            Assert.That(10m > weight, Is.True);
        }

        [Test]
        public void DecimalLessThan_Weight_ShouldWorkCorrectly()
        {
            var weight = Weight.Create(10m).Value;

            Assert.That(3m < weight, Is.True);
        }

        // task [Formatting] ------------------------------------------------------

        [Test]
        public void ToString_ShouldReturnValueWithKgUnit()
        {
            var weight = Weight.Create(5.5m).Value;

            Assert.That(weight.ToString(), Is.EqualTo("5.5 kg"));
        }

        [Test]
        public void ToString_WithWholeNumber_ShouldReturnValueWithKgUnit()
        {
            var weight = Weight.Create(10m).Value;

            Assert.That(weight.ToString(), Is.EqualTo("10 kg"));
        }

        // task [Equality] --------------------------------------------------------

        [Test]
        public void TwoWeights_WithSameValue_ShouldBeEqual()
        {
            var a = Weight.Create(5.5m).Value;
            var b = Weight.Create(5.5m).Value;

            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void TwoWeights_WithDifferentValues_ShouldNotBeEqual()
        {
            var a = Weight.Create(5.5m).Value;
            var b = Weight.Create(10m).Value;

            Assert.That(a, Is.Not.EqualTo(b));
        }
    }
}
