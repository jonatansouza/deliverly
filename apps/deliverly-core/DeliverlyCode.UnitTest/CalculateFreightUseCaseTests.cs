using DeliverlyCore.Pricing.Domain.Entities;
using DeliverlyCore.Pricing.Domain.ObjectValue;
using DeliverlyCore.Pricing.Domain.Ports;
using DeliverlyCore.Pricing.Domain.UseCases.CalculateFreight;
using DeliverlyCore.Shared.Domain;

namespace DeliverlyCode.UnitTest
{
    // ── Manual fakes ─────────────────────────────────────────────────────────

    file class FakeTariffRepository : ITariffRepository
    {
        private readonly IReadOnlyList<TariffTable> _result;
        public IEnumerable<string>? CapturedOriginPrefixes { get; private set; }
        public IEnumerable<string>? CapturedDestPrefixes { get; private set; }
        public Weight? CapturedWeight { get; private set; }

        public FakeTariffRepository(IReadOnlyList<TariffTable> result) => _result = result;

        public Task<IReadOnlyList<TariffTable>> FindPossibleMatchesAsync(
            IEnumerable<string> originPrefixes,
            IEnumerable<string> destPrefixes,
            Weight weight,
            CancellationToken ct = default)
        {
            CapturedOriginPrefixes = originPrefixes;
            CapturedDestPrefixes = destPrefixes;
            CapturedWeight = weight;
            return Task.FromResult(_result);
        }
    }

    file class FakePricingEngine : IPricingEngine
    {
        private readonly Result<Money> _result;
        public ZipCode? CapturedOrigin { get; private set; }
        public ZipCode? CapturedDestination { get; private set; }
        public Weight? CapturedWeight { get; private set; }
        public IReadOnlyList<TariffTable>? CapturedCandidates { get; private set; }

        public FakePricingEngine(Result<Money> result) => _result = result;

        public Result<Money> CalculateBestPrice(
            ZipCode origin,
            ZipCode destination,
            Weight weight,
            IReadOnlyList<TariffTable> candidates)
        {
            CapturedOrigin = origin;
            CapturedDestination = destination;
            CapturedWeight = weight;
            CapturedCandidates = candidates;
            return _result;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    [TestFixture]
    public class CalculateFreightUseCaseTests
    {
        private static ZipCode Zip(string raw) => ZipCode.Create(raw).Value;
        private static Weight Kg(decimal v) => Weight.Create(v).Value;
        private static Money Brl(decimal amount) => Money.Create(amount, "BRL").Value;

        private static TariffTable DefaultTariff() =>
            TariffTable.Create("Rule", Zip("01310100"), Zip("02010000"), Kg(1m), Kg(100m), Brl(50m)).Value;

        private static CalculateFreightRequest ValidRequest() =>
            new("01310100", "02010000", 10m);

        // ── task [TranslateInputs] ────────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_InvalidOriginZip_ReturnsFailure()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            var result = await sut.ExecuteAsync(new("INVALID", "02010000", 10m));

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task ExecuteAsync_InvalidDestinationZip_ReturnsFailure()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            var result = await sut.ExecuteAsync(new("01310100", "INVALID", 10m));

            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task ExecuteAsync_InvalidWeight_ReturnsFailure()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            var result = await sut.ExecuteAsync(new("01310100", "02010000", -1m));

            Assert.That(result.IsFailure, Is.True);
        }

        // business rule [Fail-Fast] ───────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_InvalidOriginZip_DoesNotCallRepository()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(new("INVALID", "02010000", 10m));

            Assert.That(repo.CapturedOriginPrefixes, Is.Null);
        }

        // ── task [PrepareSearch] ──────────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_ValidRequest_SendsAllPrefixesToRepository()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Failure("no match"));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(ValidRequest());

            var expectedOriginPrefixes = Zip("01310100").GetAllPrefixes().ToList();
            var expectedDestPrefixes = Zip("02010000").GetAllPrefixes().ToList();

            Assert.That(repo.CapturedOriginPrefixes, Is.EquivalentTo(expectedOriginPrefixes));
            Assert.That(repo.CapturedDestPrefixes, Is.EquivalentTo(expectedDestPrefixes));
        }

        // ── task [Fetch] ──────────────────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_ValidRequest_PassesWeightToRepository()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Failure("no match"));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(ValidRequest());

            Assert.That(repo.CapturedWeight!.Value, Is.EqualTo(10m));
        }

        // ── task [DelegateCalculation] ────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_ValidRequest_PassesCandidatesToEngine()
        {
            var tariff = DefaultTariff();
            var repo = new FakeTariffRepository([tariff]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(ValidRequest());

            Assert.That(engine.CapturedCandidates, Has.Count.EqualTo(1));
            Assert.That(engine.CapturedCandidates![0], Is.SameAs(tariff));
        }

        [Test]
        public async Task ExecuteAsync_ValidRequest_PassesVOsToEngine()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Failure("no match"));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(ValidRequest());

            Assert.That(engine.CapturedOrigin!.Value, Is.EqualTo("01310100"));
            Assert.That(engine.CapturedDestination!.Value, Is.EqualTo("02010000"));
            Assert.That(engine.CapturedWeight!.Value, Is.EqualTo(10m));
        }

        // business rule [Pure Orchestration] ─────────────────────────────────

        [Test]
        public async Task ExecuteAsync_DoesNotFilterOrSortCandidatesBeforeEngine()
        {
            var tariff1 = DefaultTariff();
            var tariff2 = DefaultTariff();
            var repo = new FakeTariffRepository([tariff1, tariff2]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(50m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            await sut.ExecuteAsync(ValidRequest());

            // all candidates from repository reach the engine unfiltered
            Assert.That(engine.CapturedCandidates, Has.Count.EqualTo(2));
        }

        // ── task [HandleFailure] ──────────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_WhenEngineReturnsFailure_ReturnsFailure()
        {
            var repo = new FakeTariffRepository([]);
            var engine = new FakePricingEngine(Result<Money>.Failure("No tariff matches the given route and weight."));
            var sut = new CalculateFreightUseCase(repo, engine);

            var result = await sut.ExecuteAsync(ValidRequest());

            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("No tariff matches the given route and weight."));
        }

        // ── task [Return] ─────────────────────────────────────────────────────

        [Test]
        public async Task ExecuteAsync_ValidRequest_ReturnsMappedFreightResponse()
        {
            var repo = new FakeTariffRepository([DefaultTariff()]);
            var engine = new FakePricingEngine(Result<Money>.Success(Brl(75m)));
            var sut = new CalculateFreightUseCase(repo, engine);

            var result = await sut.ExecuteAsync(ValidRequest());

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value.OriginZip, Is.EqualTo("01310100"));
            Assert.That(result.Value.DestinationZip, Is.EqualTo("02010000"));
            Assert.That(result.Value.WeightKg, Is.EqualTo(10m));
            Assert.That(result.Value.Price, Is.EqualTo(75m));
            Assert.That(result.Value.Currency, Is.EqualTo("BRL"));
        }
    }
}
