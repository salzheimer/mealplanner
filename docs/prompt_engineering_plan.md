# Meal Planning Microservices — Prompt Engineering Plan

This document captures the implementation plan for the Meal Planning microservices system, written in a way that is easy to follow and use as a prompt-engineering reference.

## Core Requirements
- **Framework:** .NET 10 (latest LTS at time of writing)
- **Service Architecture:** microservices with clear boundaries
- **Authentication:** JWT tokens issued by AuthService
- **Inter-service Communication:** REST/HTTP (Docker Compose DNS)
- **API Contract Approach:** Scalar (instead of Swagger) for API contract generation and prompt-engineering-friendly documentation
- **Database:** Shared PostgreSQL (existing schema in `infrastructure/postgres/init.sql`)

---

## Project Structure (Prompt Engineering Friendly)

```
mealplanner/
├── docs/
│   ├── meal_planning_datamodel.drawio
│   ├── architecture.drawio           # Service interaction / architecture diagrams
│   ├── prompt_engineering_plan.md    # This file (prompt-design friendly requirements + plan)
│   └── adr/                         # Architectural Decision Records
├── infrastructure/
│   ├── docker/
│   │   └── docker-compose.yml       # Orchestration for all services + postgres
│   └── postgres/
│       └── init.sql                 # Base schema used by services
├── services/
│   ├── ApiGateway/                  # API gateway (optional but recommended)
│   ├── AuthService/                 # JWT + user management
│   ├── MealService/                 # Recipes, ingredients, meal scheduling
│   └── PlanService/                 # Meal planning / plan + meal item tracking
├── shared/                          # Shared DTOs, enums, helpers
│   ├── Shared.Models/
│   ├── Shared.Services/
│   └── Shared.Tests/
└── frontend/
    └── meal-planner-react/
```

---

## Service Responsibilities

### AuthService
- Register & authenticate users
- Issue JWT access tokens
- Optionally provide token validation endpoint for internal use

### MealService
- Manage recipes, ingredients, and instructions
- Manage meals (breakfast/lunch/dinner/snack)
- Provide search + filtering + bulk import endpoints
- Provide lightweight analytics (popularity, ranking)

### PlanService
- Create/update/delete meal plans (date range, user)
- Manage meal items within a plan (assignment + status)
- Provide plan analytics/summary endpoints

### ApiGateway (recommended)
- Route requests to individual services
- Centralize JWT validation and rate limiting
- Provide unified Scalar-generated API contract

---

## API Contract / Documentation Strategy

- Use **Scalar** instead of Swagger to generate API contract documentation in a format optimized for prompt engineering and model-driven code generation.
- Store Scalar output and contract artifacts alongside service code so LLMs can reference them directly.

---

## Additional Recommendations (Prompt-Friendly)

- **Document everything as code**: keep architecture diagrams, decision records, and API contracts in repo so automated tooling (LLMs, CI, docs generators) can use them.
- **Use .NET 10** everywhere for consistency.
- **Use a shared `Shared.Models` project** for enumerations, DTOs, and contract definitions so prompt-based generation can reuse consistent structures.
- **Make health endpoints explicit** (`/health`, `/ready`) so orchestration and model-driven automation can verify service readiness.

---

## Notes for Prompt Engineering

When generating code or guidance via prompts, reference:
- `docs/prompt_engineering_plan.md` for architecture & boundaries
- Scalar API contracts for request/response expectations
- `infrastructure/docker/docker-compose.yml` for service names and ports
- `infrastructure/postgres/init.sql` for database schema

Keep prompts aligned with this plan to ensure consistent, deterministic outputs.