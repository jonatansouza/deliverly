using DeliverlyCore.Pricing.Domain.ObjectValue;

namespace DeliverlyCode.UnitTest
{
    [TestFixture]
    public class ZipCodeTests
    {
        // task [Create]
        [Test]
        public void Create_WithValidDigitsOnly_ShouldSucceed()
        {
            var result = ZipCode.Create("01310100");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Value, Is.EqualTo("01310100"));
        }

        [Test]
        public void Create_WithDash_ShouldStripAndSucceed()
        {
            var result = ZipCode.Create("01310-100");

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.Value, Is.EqualTo("01310100"));
        }

        // task [Validate]
        [Test]
        public void Create_WithNull_ShouldReturnFailure()
        {
            var result = ZipCode.Create(null!);

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithEmpty_ShouldReturnFailure()
        {
            var result = ZipCode.Create(string.Empty);

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithFewerThan8Digits_ShouldReturnFailure()
        {
            var result = ZipCode.Create("1234567");

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithMoreThan8Digits_ShouldReturnFailure()
        {
            var result = ZipCode.Create("123456789");

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public void Create_WithNonNumericChars_ShouldReturnFailure()
        {
            var result = ZipCode.Create("0131010A");

            Assert.That(result.IsFailure, Is.True);
        }

        // task [Formatting]
        [Test]
        public void ToFormattedString_ShouldReturnPattern00000_000()
        {
            var zipCode = ZipCode.Create("01310100").Value;

            Assert.That(zipCode.ToFormattedString(), Is.EqualTo("01310-100"));
        }

        // business rule [Equality]
        [Test]
        public void TwoZipCodes_WithSameValue_ShouldBeEqual()
        {
            var a = ZipCode.Create("01310100").Value;
            var b = ZipCode.Create("01310100").Value;

            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void TwoZipCodes_WithDifferentValues_ShouldNotBeEqual()
        {
            var a = ZipCode.Create("01310100").Value;
            var b = ZipCode.Create("04538133").Value;

            Assert.That(a, Is.Not.EqualTo(b));
        }
    }
}
