# Spec: Entity - TariffTable

## 1. Description

Defines the base freight cost between two postal regions identified by Zip Code prefixes and weight ranges.

## 2. Business Rules (Implementation Checklist)

- [ ] Prefix Match: The calculation must identify the tariff where OriginPrefix and DestinationPrefix match the start of the provided Zip Codes.
- [ ] Weight Range: Must support weight brackets (e.g., 0kg to 5kg is one price, 5kg to 10kg is another).
- [ ] Currency: The value must be handled as a Money Value Object (avoid using double or float for financial values).
- [ ] Exclusivity: A single combination of prefixes and weight range should be unique to avoid pricing conflicts.

## 3. Data Schema

- `Id`: Guid
- `OriginPrefix`: string (ex: "010")
- `DestinationPrefix`: string (ex: "200")
- `MinWeight`: decimal
- `MaxWeight`: decimal
- `BaseValue`: Money

## 4. Expected Behaviors (Tasks)

- [ ] task [Match]: Create a method IsEligible(ZipCode origin, ZipCode destination, decimal weight).
- [ ] task [PriceCalculation]: Return the BaseValue if all criteria in IsEligible are met.
- [ ] task [Validation]: Ensure MinWeight is always less than MaxWeight during creation or update.
