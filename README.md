# Deliverly 📦

**Deliverly** is a high-performance delivery orchestration platform built using a monorepo architecture. It leverages distributed services to handle complex logistics, real-time pricing calculations, and reliable transaction processing.

---

## 🚀 Architecture & Tech Stack

This project is managed as an **Nx Monorepo**, allowing for seamless code sharing and task orchestration across the entire stack.

### Apps

- **deliverly-front:** A modern web interface built with **React**, TypeScript, and Tailwind CSS.
- **deliverly-back:** A robust **NestJS** backend serving as the main API and orchestration layer.
- **deliverly-core:** A high-performance **.NET 9** engine dedicated to complex pricing and core business logic.

### Infrastructure & Services

- **Messaging & Events:** **Apache Kafka** for reliable asynchronous communication between services.
- **Caching:** **Redis** for low-latency data access and distributed state management.
- **Databases:** A hybrid approach using **PostgreSQL** for relational data and **CouchDB** for document-oriented storage.
- **Containerization:** Orchestrated via **Docker Compose** for consistent development and deployment environments.

---

## ✨ Key Features

- **Idempotency Integration:** Utilizes the **Idempotency Key Pattern** for critical operations like ticket creation to ensure system reliability.
- **Dynamic Pricing Engine:** Offloads heavy pricing calculations to the dedicated `.NET` core service.
- **Monorepo Efficiency:** Powered by **Nx**, enabling optimized builds, testing, and shared libraries across the frontend and backend.
- **Calculations & History:** Dedicated modules for real-time delivery estimations and comprehensive transaction tracking.

---

## 🛠️ Project Structure

```text
DELIVERLY/
├── apps/
│   ├── deliverly-back/     # NestJS Backend API
│   ├── deliverly-core/     # .NET 9 Pricing Engine
│   └── deliverly-front/    # React Frontend
├── packages/               # Shared utilities and libraries
├── docker-compose.yaml     # Infrastructure orchestration
├── nx.json                 # Nx Monorepo configuration
├── package.json            # Workspace dependencies
└── tsconfig.base.json      # Base TypeScript configuration
```
