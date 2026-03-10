# Spec: Use Case - CalculateFreight

## 1. Description

Orchestrates the freight calculation. It acts as a pure coordinator: translating inputs to Domain Objects, delegating the calculation to the Domain Service (`IPricingEngine`), and coordinating the Kafka side-effect (`IEventBus`).

## 2. Business Rules (Implementation Checklist)

- [ ] **Pure Orchestration:** The Use Case must contain ZERO routing, filtering, or sorting logic. All business rules are delegated.
- [ ] **Fail-Fast:** Return `Result.Failure` immediately if Value Object creation fails.
- [ ] **Side-Effects:** Must publish a `FreightCalculatedEvent` to the Event Bus ONLY if the calculation is successful.

## 3. Dependencies (Ports)

- `ITariffRepository`: To fetch active tariffs.
- `IPricingEngine`: (Domain Service) To execute the Longest Prefix Match calculation.

## 4. Expected Behaviors (Tasks)

- [ ] `task [TranslateInputs]`: Convert string inputs to `ZipCode` and decimal to `Money`/`Weight` (VOs). Return failure if invalid.
- [ ] `task [PrepareSearch]`: Extrair as listas de prefixos usando `originZip.GetAllPrefixes()` e `destinationZip.GetAllPrefixes()`.
- [ ] `task [Fetch]`: Aguardar `ITariffRepository.FindPossibleMatchesAsync(originPrefixes, destPrefixes, weight, ct)`. O repositório retornará apenas um pequeno subconjunto de tarifas candidatas.
- [ ] `task [DelegateCalculation]`: Passar essa lista enxuta para o `IPricingEngine` aplicar a regra de "Longest Prefix Match" (SpecificityScore) em memória e retornar o vencedor.
- [ ] `task [Fetch]`: Await `ITariffRepository.GetActiveTariffsAsync()` parameters should be origin and destination prefix.
- [ ] `task [DelegateCalculation]`: Pass the VOs and the tariffs to `IPricingEngine.CalculateBestPrice(origin, destination, weight, tariffs)`.
- [ ] `task [HandleFailure]`: If the engine returns a failure (no match), return that failure immediately.
- [ ] `task [Publish]`: Instantiate `FreightCalculatedEvent` and await `IEventBus.PublishAsync()`.
- [ ] `task [Return]`: Map the Domain result to `FreightResponse` and return.
