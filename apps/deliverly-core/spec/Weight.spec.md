# Spec: Value Object - Weight

## 1. Description

Represents the physical mass of a package or cargo used to determine the applicable freight tariff bracket. It standardizes the measurement unit (Kilograms) and ensures physical constraints.

## 2. Business Rules (Implementation Checklist)

- [x] **Precision:** Must use `decimal` to maintain consistency and avoid precision loss when interacting with `Money` or pricing rules.
- [x] **Physical Constraint:** Weight cannot be negative and cannot be zero (a physical package must have some mass, even if it's 0.01 kg for documents).
- [x] **Immutability:** The value cannot be modified after instantiation.
- [x] **Unit Standardization:** The system assumes the internal value is always in Kilograms (kg).

## 3. Data Schema

- `Value`: `decimal` (Represents the weight in kg).

## 4. Expected Behaviors (Tasks)

- [x] `task [Create]`: Static factory method `Create(decimal value)`.
- [x] `task [Validate]`: Return `Result.Failure` if `value <= 0`.
- [x] `task [ComparisonOperators]`: Overload relational operators (`>`, `<`, `>=`, `<=`) to allow seamless comparison with `MinWeight` and `MaxWeight` decimals in `TariffTable`.
- [x] `task [Equality]`: Two `Weight` objects are equal if their `Value` is exactly the same.
- [x] `task [Formatting]`: Method `ToString()` should return the formatted string with the unit (e.g., "5.5 kg").
