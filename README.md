# Deliverly

**Deliverly** is a delivery orchestration and freight pricing platform built on a polyglot microservices monorepo. It combines a .NET 9 pricing engine with a NestJS orchestration layer and a React frontend, connected through Apache Kafka and designed from day one with **spec-driven development** and **Domain-Driven Design** principles.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                  deliverly-front (React)                │
│          Freight Calculator  ·  History View            │
└────────────────────────┬────────────────────────────────┘
                         │ HTTP
┌────────────────────────▼────────────────────────────────┐
│               deliverly-back (NestJS)                   │
│    CQRS  ·  Idempotency  ·  CouchDB  ·  Kafka Publish  │
└─────────────┬───────────────────────┬───────────────────┘
              │ Kafka Events          │ HTTP
┌─────────────▼───────────────────────▼───────────────────┐
│             deliverly-core (.NET 9)                     │
│  DDD  ·  Clean Architecture  ·  Longest-Prefix-Match   │
│  EF Core 8  ·  PostgreSQL  ·  Result Pattern           │
└─────────────────────────────────────────────────────────┘
```

**Monorepo:** Nx workspace with unified build orchestration across Node.js and .NET projects.

---

## Services

### deliverly-core — .NET 9 Freight Pricing Engine

The most architecturally rich service in the platform. Built with **Clean Architecture** and **Domain-Driven Design**, it exposes a RESTful API for tariff management and freight price calculation.

**Architecture layers:**

| Layer | Project | Responsibility |
|---|---|---|
| API | `DeliverlyCore` | ASP.NET Core controllers, DI wiring |
| Domain | `DeliverlyCore.Domain` | Entities, value objects, use cases, ports |
| Infrastructure | `DeliverlyCore.Infra` | EF Core repositories, PostgreSQL |
| Shared | `DeliverlyCore.Shared` | `Result<T>`, base `Entity`, `ValueObject` |
| Tests | `DeliverlyCode.UnitTest` | Domain unit tests |

**Domain model highlights:**

- **`TariffTable`** — Aggregate root representing a freight pricing rule. Encapsulates origin/destination ZIP prefix matching, weight range validation (`MinWeight < MaxWeight`), and a base `Money` value. Created through a static factory (`Create`) and reconstituted separately from persistence (`Reconstitute`), enforcing invariants at every boundary.

- **`ZipCode`** — Value object that supports both full 8-digit Brazilian postal codes and 1–7 digit prefixes. Implements `GetAllPrefixes()` to generate every prefix variant of a full code, powering the longest-prefix-match algorithm in tariff selection.

- **`Money`** — Immutable value object using `decimal` (no floating-point errors). Supports arithmetic across ISO 4217 currencies with culture-aware formatting (BRL, USD, EUR).

- **`Weight`** — Value object enforcing positive-only values in kilograms, with comparison operators for bracket matching.

**Freight calculation — Longest-Prefix-Match:**

When a freight quote is requested, the engine:
1. Generates all ZIP prefix variants from origin and destination full codes
2. Queries the repository for all potentially matching `TariffTable` entries
3. Ranks candidates by `SpecificityScore` (longer prefix = higher specificity)
4. Selects the most specific match and computes the price via `IPricingEngine`

This mirrors how real carrier pricing works: national fallback rules yield to regional, then sector-level, then neighborhood-level tariffs.

**Error handling — Result Pattern:**

No exceptions on the happy path. All use case returns are typed as `Result<T>`, carrying either a `Value` or an `Error` string. Controllers inspect `IsSuccess` and map accordingly — a clean, railway-oriented approach.

**Endpoints:**

```
GET    /api/tariff-tables           List all tariff rules
GET    /api/tariff-tables/{id}      Get tariff by ID
POST   /api/tariff-tables           Create single tariff
POST   /api/tariff-tables/batch     Bulk insert (optimized)
PUT    /api/tariff-tables/{id}      Update tariff
DELETE /api/tariff-tables/{id}      Delete tariff
POST   /api/freight/calculate       Calculate freight price
```

**Spec-Driven Development:**

API contracts and tariff scenarios are defined as `.http` files (`DeliverlyCore.http`) before implementation. The spec covers a full tariff hierarchy across real Brazilian delivery zones (São Paulo → Rio de Janeiro, São Paulo → Belo Horizonte, intra-state routes), with multiple weight brackets at each geographic level. These specs drive both development and serve as living documentation — readable, executable, and version-controlled alongside the code.

---

### deliverly-back — NestJS Orchestration Layer

The Node.js backend handles ticket lifecycle management using **CQRS** and publishes domain events to Kafka.

**Patterns applied:**
- **CQRS** — `TicketUpdateCommand` (write to CouchDB) and `TicketPublishCommand` (emit Kafka event) are distinct command handlers. `TicketQuery` handles reads.
- **Idempotency Pattern** — Before creating a ticket, the service checks for an existing document by ID. Duplicate requests return the existing ticket instead of creating a second one.
- **Repository Abstraction** — `CouchDbTicketRepository` implements the abstract `TicketRepository`, keeping domain logic decoupled from the database driver.

**Tech:** NestJS 11 · CQRS module · CouchDB (Nano) · Apache Kafka · TypeScript

---

### deliverly-front — React Frontend

Glassmorphism-styled SPA for freight calculation and history tracking.

**Tech:** React 19 · TypeScript · Vite · Tailwind CSS v4 · React Router v7 · i18next (i18n)

---

## Tech Stack

| Concern | Technology |
|---|---|
| Frontend | React 19, TypeScript, Tailwind CSS v4, Vite |
| Backend API | NestJS 11, TypeScript, CQRS |
| Pricing Engine | .NET 9, ASP.NET Core, EF Core 8 |
| Document DB | CouchDB |
| Relational DB | PostgreSQL 16 |
| Cache | Redis Stack (with RedisInsight GUI on :8001) |
| Messaging | Apache Kafka (KRaft, no ZooKeeper) |
| Monitoring | Kafka UI |
| Containerization | Docker Compose |
| Monorepo | Nx |

---

## Running Locally

**Prerequisites:** Docker, Docker Compose, Node.js, .NET 9 SDK

```bash
# Start all infrastructure and services
docker compose up -d

# Or run services individually for development:
# Backend (NestJS) — port 3000
cd apps/deliverly-back && npm install && npm run start:dev

# Pricing Engine (.NET) — port 5000
cd apps/deliverly-core && dotnet run --project DeliverlyCore

# Frontend (React) — port 5173
cd apps/deliverly-front && npm install && npm run dev
```

**Service URLs:**

| Service | URL |
|---|---|
| Frontend | http://localhost:5173 |
| Backend API | http://localhost:3000 |
| Pricing Engine | http://localhost:5000 |
| CouchDB | http://localhost:5984 |
| Redis | http://localhost:6379 |
| RedisInsight | http://localhost:8001 |
| Kafka | localhost:9092 |
| Kafka UI | http://localhost:8080 |
| PostgreSQL | localhost:5432 |

---

## Project Structure

```
deliverly/
├── apps/
│   ├── deliverly-back/           # NestJS orchestration API
│   ├── deliverly-core/           # .NET 9 pricing engine
│   │   ├── DeliverlyCore/        # Web API + .http spec files
│   │   ├── DeliverlyCore.Domain/ # Entities, value objects, use cases
│   │   ├── DeliverlyCore.Infra/  # EF Core + PostgreSQL
│   │   ├── DeliverlyCore.Shared/ # Result<T>, base types
│   │   └── DeliverlyCode.UnitTest/
│   └── deliverly-front/          # React SPA
├── packages/                     # Shared libraries (Nx workspace)
├── docker-compose.yaml
├── nx.json
└── package.json
```

---

## Key Engineering Decisions

- **Spec-first with `.http` files** — API contracts are written as executable HTTP specs before implementation, enabling contract-first development without heavy tooling.
- **Longest-prefix-match tariff selection** — Mirrors real carrier pricing systems; more specific ZIP prefix rules win over broader fallback rules.
- **`decimal` for money, not `double`** — Avoids floating-point precision bugs in financial calculations.
- **Result pattern over exceptions** — Domain logic never throws; callers handle `Result<T>` explicitly.
- **Hybrid databases** — PostgreSQL for structured tariff data (relational, EF Core migrations), CouchDB for semi-structured ticket documents (flexible schema, document semantics).
- **Kafka in KRaft mode** — No ZooKeeper dependency; simpler operational footprint.
- **CQRS + Idempotency** — Prevents duplicate ticket creation under network retries without distributed locks.
