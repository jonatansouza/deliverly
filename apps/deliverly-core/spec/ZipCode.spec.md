# Spec: Value Object - ZipCode

## 1. Description

Represent a Brazilian valid zip code

## 2. Business Rules (Implementation Checklist)

[X] Format: Must contain exactly 8 numeric digits.
[X] Normalization: Must strip masks (hyphens, dots) before storing; only numbers are persisted.
[X] Validation: Must not be null, empty, or have a length other than 8 digits.
[X] Equality: Comparison must be based on the numeric value (Value Object behavior).
[X] Immutability: The value cannot be changed after instantiation.

## 3. Data Schema

- `Value`: string (8 chars, numeric only)

## 4. Expected Behaviors (Tasks)

[X] task [Create]: Method Create(string rawZipCode). Must handle inputs with or without hyphens (e.g., "04145-000" or "04145000").
[X] task [Validate]: Return Result.Failure if the input is non-numeric or does not meet the 8-digit requirement after cleaning.
[X] task [Formatting]: Method ToFormattedString() returns the pattern 00000-000.
[X] task [GetRegion]: Method GetRegion() returns the first digit (e.g., "0").
[X] task [GetSubRegion]: Method GetSubRegion() returns the first two digits (e.g., "04").
[X] task [GetSector]: Method GetSector() returns the first five digits (e.g., "04145").
[X] task [Comparison]: Method IsSameSector(ZipCode other) to identify if two addresses belong to the same micro-zone.
[X] task [Comparison]: Method IsSameRegion(ZipCode other) to identify if two addresses belong to the same macro-region.
