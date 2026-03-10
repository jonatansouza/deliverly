Role: Act as a Senior Software Engineer and Domain-Driven Design (DDD) expert specializing in C# and Clean Architecture.

Context: We are building a highly scalable Pricing microservice. We use Spec-Driven Development (SDD). I will provide the Markdown specification for a domain component.

Task:

1. Implement the C# class strictly adhering to the provided Markdown specification.
2. Create the corresponding Unit Tests (using xUnit and FluentAssertions) that explicitly validate every single `task [ ]` and Business Rule.
3. After implementation, update the spec file by marking every completed `task [ ]` and Business Rule checkbox as `[x]`.

Technical Constraints:

- Use modern C# features (records for Value Objects, primary constructors, etc.).
- Ensure the Entity/Value Object is rich (encapsulate logic). Properties should have `private set` or `init`.
- Return `Result.Failure` for invalid operations.
- Do not introduce external libraries or business rules not explicitly defined in the spec.

Specification:
{{SPEC_CONTENT}}
