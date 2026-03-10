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

        // business rule [Immutability]
        [Test]
        public void ZipCode_Value_ShouldHaveNoPublicSetter()
        {
            var property = typeof(ZipCode).GetProperty(nameof(ZipCode.Value));

            Assert.That(property!.CanWrite, Is.False);
        }

        // task [GetRegion]
        [Test]
        public void GetRegion_ShouldReturnFirstDigit()
        {
            var zipCode = ZipCode.Create("04145000").Value;

            Assert.That(zipCode.GetRegion(), Is.EqualTo("0"));
        }

        // task [GetSubRegion]
        [Test]
        public void GetSubRegion_ShouldReturnFirstTwoDigits()
        {
            var zipCode = ZipCode.Create("04145000").Value;

            Assert.That(zipCode.GetSubRegion(), Is.EqualTo("04"));
        }

        // task [GetSector]
        [Test]
        public void GetSector_ShouldReturnFirstFiveDigits()
        {
            var zipCode = ZipCode.Create("04145000").Value;

            Assert.That(zipCode.GetSector(), Is.EqualTo("04145"));
        }

        // task [Comparison] IsSameSector
        [Test]
        public void IsSameSector_WithSameSector_ShouldReturnTrue()
        {
            var a = ZipCode.Create("04145000").Value;
            var b = ZipCode.Create("04145999").Value;

            Assert.That(a.IsSameSector(b), Is.True);
        }

        [Test]
        public void IsSameSector_WithDifferentSector_ShouldReturnFalse()
        {
            var a = ZipCode.Create("04145000").Value;
            var b = ZipCode.Create("04146000").Value;

            Assert.That(a.IsSameSector(b), Is.False);
        }

        // task [Comparison] IsSameRegion
        [Test]
        public void IsSameRegion_WithSameRegion_ShouldReturnTrue()
        {
            var a = ZipCode.Create("04145000").Value;
            var b = ZipCode.Create("01310100").Value;

            Assert.That(a.IsSameRegion(b), Is.True);
        }

        [Test]
        public void IsSameRegion_WithDifferentRegion_ShouldReturnFalse()
        {
            var a = ZipCode.Create("04145000").Value;
            var b = ZipCode.Create("30130010").Value;

            Assert.That(a.IsSameRegion(b), Is.False);
        }
    }
}
