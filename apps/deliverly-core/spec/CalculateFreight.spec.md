# Spec: Use Case - CalculateFreight

## 1. Description

Orchestrates the freight calculation. It acts as a pure coordinator: translating inputs to Domain Objects, delegating the calculation to the Domain Service (`IPricingEngine`)

## 2. Business Rules (Implementation Checklist)

- [x] **Pure Orchestration:** The Use Case must contain ZERO routing, filtering, or sorting logic. All business rules are delegated.
- [x] **Fail-Fast:** Return `Result.Failure` immediately if Value Object creation fails.

## 3. Dependencies (Ports)

- `ITariffRepository`: To fetch active tariffs.
- `IPricingEngine`: (Domain Service) To execute the Longest Prefix Match calculation.

## 4. Expected Behaviors (Tasks)

- [x] `task [TranslateInputs]`: Convert string inputs to `ZipCode` and decimal to `Money`/`Weight` (VOs). Return failure if invalid.
- [x] `task [PrepareSearch]`: extract prefixes `originZip.GetAllPrefixes()` e `destinationZip.GetAllPrefixes()`.
- [x] `task [Fetch]`: Find `ITariffRepository.FindPossibleMatchesAsync(originPrefixes, destPrefixes, weight, ct)`. return the candidates for match.
- [x] `task [DelegateCalculation]`: delegate execution to `IPricingEngine` "Longest Prefix Match" (SpecificityScore) in memory.
- [x] `task [HandleFailure]`: If the engine returns a failure (no match), return that failure immediately.
- [x] `task [Return]`: Map the Domain result to `FreightResponse` and return.
