# Meal Planner Monorepo

This repository contains a meal planner application built with a .NET microservices backend, Postgres database, and a React frontend.

## Structure

```
mealplanner/
├── services/            # .NET microservices
│   ├── MealService/
│   │   ├── src/
│   │   └── tests/
│   ├── OrderService/
│   │   ├── src/
│   │   └── tests/
│   └── AuthService/
│       ├── src/
│       └── tests/
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

- Each microservice is a standalone .NET solution. Use `dotnet new` to scaffold inside the `src` directories.
- Frontend is a standard Create React App or Vite project in `frontend/meal-planner-react`.
- PostgreSQL initialization scripts live under `infrastructure/postgres`.
- Docker compose file in `infrastructure/docker` orchestrates local development.
