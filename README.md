# Meal Planner

The purpose of this project is to create an application to make meal planning easier. Also to practice with AI assisted coding.

This repository contains a meal planner application built with a .NET 10 microservices backend, Postgres database, and a React frontend.

## Structure

```
mealplanner/
├── services/            # .NET microservices
│   ├── ApiGateway/      # Optional API gateway (routing + central auth)
│   │   ├── src/
│   │   └── tests/
│   ├── IdentityService/     # JWT auth + user management
│   │   ├── src/
│   │   └── tests/
│   ├── MealService/     # Recipes, ingredients, meal scheduling
│   │   ├── src/
│   │   └── tests/
│   └── PlanService/     # Meal planning + meal item assignment
│       ├── src/
│       └── tests/
├── shared/              # Shared DTOs, enums, helpers
│   ├── Shared.Models/
│   ├── Shared.Services/
│   └── Shared.Tests/
├── frontend/            # React application
│   └── meal-planner-react/
│       └── src/
├── infrastructure/      # Deployment and tooling
│   ├── docker/
│   │   └── docker-compose.yml
│   └── postgres/
│       └── init.sql
└── docs/                # Project documentation
    └── adr/             # Architectural Decision Records
```

## Getting Started

### Run locally with Docker
1. Create the external volume (once, before first run):
   ```bash
   docker volume create mealplanningdb_data
   ```
2. Start services:
   ```bash
   docker compose -f infrastructure/docker/docker-compose.yml up --build
   ```
3. Access services:
   - ApiGateway: http://localhost:5001
   - IdentityService: http://localhost:5002
   - MealService: http://localhost:5003
   - PlanService: http://localhost:5004

### Run a single service (dotnet)
1. `cd services/IdentityService/src`
2. `dotnet run`

### Notes
- API contract generation is intended to use Scalar (see `docs/scalar.md`).
- JWT secrets are in `appsettings.json` for development; replace with a secure secret in production.
