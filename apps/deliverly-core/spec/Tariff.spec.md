# Spec: Entity - TariffTable

## 1. Description

Defines the base freight cost between two postal regions using the "Longest Prefix Match" strategy. This allows for broad rules (state-to-state) and specific overrides (city-to-city) without redundant data.

## 2. Business Rules (Implementation Checklist)

- [x] **Longest Prefix Match:** When multiple rules match a pair of ZipCodes, the one with the longest prefixes (most specific) must take precedence.
- [x] **Weight Brackets:** The rule only applies if the cargo weight falls within `MinWeight` and `MaxWeight`.
- [x] **Currency Consistency:** All financial values must use the `Money` Value Object.
- [x] **Data Integrity:** `OriginPrefix` and `DestinationPrefix` should only contain numbers (sanitized).

## 3. Data Schema

- `Id`: Guid (Identity)
- `Description`: string (e.g., "General Southeast to South" or "Downtown SP to Downtown RJ")
- `OriginPrefix`: string (1 to 5 digits)
- `DestinationPrefix`: string (1 to 5 digits)
- `MinWeight`: decimal
- `MaxWeight`: decimal
- `BaseValue`: Money (Value Object)

## 4. Expected Behaviors (Tasks)

- [x] `task [Eligibility]`: Method `IsMatch(string originZip, string destinationZip, decimal weight)`.
  - Checks if `originZip.StartsWith(OriginPrefix)`.
  - Checks if `destinationZip.StartsWith(DestinationPrefix)`.
  - Checks if weight is within range.
- [x] `task [SpecificityScore]`: Create a property or method that returns the combined length of both prefixes (`OriginPrefix.Length + DestinationPrefix.Length`). This will be used to rank the best rule.
- [x] `task [Validation]`: Ensure that during creation, prefixes do not contain hyphens or spaces.
