# Meal Planner Monorepo

This repository contains a meal planner application built with a .NET 10 microservices backend, Postgres database, and a React frontend.

## Structure

```
mealplanner/
├── services/            # .NET microservices
│   ├── ApiGateway/      # Optional API gateway (routing + central auth)
│   │   └── src/
│   ├── AuthService/     # JWT auth + user management
│   │   └── src/
│   ├── MealService/     # Recipes, ingredients, meal scheduling
│   │   └── src/
│   └── PlanService/     # Meal planning + meal item assignment
│       └── src/
├── shared/              # Shared DTOs, enums, helpers
│   ├── Shared.Models/
│   └── Shared.Services/
├── frontend/            # React application
│   └── meal-planner-react/
│       └── src/
├── infrastructure/      # Deployment and tooling
│   ├── docker/
│   │   └── docker-compose.yml
│   └── postgres/
│       └── init.sql
└── docs/                # Project documentation
```

## Getting Started

### Run locally with Docker
1. Start services:
   ```bash
   docker compose -f infrastructure/docker/docker-compose.yml up --build
   ```
2. Access services:
   - AuthService: http://localhost:5001
   - MealService: http://localhost:5002
   - PlanService: http://localhost:5003
   - ApiGateway: http://localhost:5000

### Run a single service (dotnet)
1. `cd services/AuthService/src`
2. `dotnet run`

### Notes
- API contract generation is intended to use Scalar (see `docs/scalar.md`).
- JWT secrets are in `appsettings.json` for development; replace with a secure secret in production.
