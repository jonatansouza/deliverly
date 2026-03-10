# Spec: Value Object - ZipCode

## 1. Description

Represent a Brazilian valid zip code

## 2. Business Rules (Implementation Checklist)

- [x] **Format:** Should contains 8 digits only.
- [x] **Normalization:** should contains - on view.
- [x] **Validation:** do not be empty or null.
- [x] **Equality:** Two values should be the same if has same value.

## 3. Data Schema

- `Value`: string (8 chars, numeric only)

## 4. Expected Behaviors (Tasks)

- [x] `task [Create]`: Instance `Create(string rawZipCode)` (can receive with - or without, and should know to save only numbers).
- [x] `task [Validate]`: Return `Result.Failure` if the format is invalid.
- [x] `task [Formatting]`: Method `ToFormattedString()` return the pattern `00000-000`.
