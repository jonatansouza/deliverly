# Spec: Value Object - Money

## 1. Description

Handles financial values with high precision, preventing floating-point errors and ensuring currency consistency during logistics pricing calculations.

## 2. Business Rules (Implementation Checklist)

- [x] **Precision:** Must use `decimal` for the amount to avoid binary floating-point inaccuracies.
- [x] **Immutability:** Once created, the amount and currency cannot be modified (Value Object pattern).
- [x] **Currency Matching:** Arithmetic operations (Addition/Subtraction) can only be performed between the same currency.
- [x] **Validation:** The amount must not be negative for freight prices.
- [x] **Equality:** Two Money objects are equal only if both the `Amount` and `Currency` are identical.

## 3. Data Schema

- `Amount`: `decimal` (The numerical value of the price).
- `Currency`: `string` (ISO 4217 code, e.g., "BRL", "USD").

## 4. Expected Behaviors (Tasks)

- [x] `task [Create]`: Factory method `Create(decimal amount, string currency)`. Validates against negative values.
- [x] `task [Arithmetic]`: Implement `Add(Money other)` and `Subtract(Money other)`. Return `Result.Failure` if currencies differ.
- [x] `task [Multiply]`: Method `Multiply(decimal factor)` to apply percentage-based rules (e.g., fuel surcharges).
- [x] `task [Formatting]`: Method `ToString()` or `ToFormattedString()` to return a human-readable format (e.g., "R$ 10,00").
