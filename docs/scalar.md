# Scalar API Contract Strategy

This project uses **Scalar** as the canonical API contract system instead of relying solely on Swagger/OpenAPI UI.

## What is Scalar?
Scalar is a contract-driven API specification tool that enables:
- API contract generation in a form suited for prompt-engineered code generation
- Strong typing for request/response models
- Generation of SDKs and documentation from a single source of truth

> **Note:** This repo retains OpenAPI/OpenAPI metadata in each service for local tooling and developer convenience. Scalar is the primary contract used for prompt-driven code generation and integration tests.

## Where to put Scalar files
Each service includes a `scalar.yml` file at its repository root (e.g., `services/IdentityService/src/scalar.yml`).

The root `scalar.yml` can be used to generate combined contracts across all services.

## How to use
1. Install Scalar CLI (project-specific directions will vary).
2. Run:
   ```bash
   scalar generate --config services/IdentityService/src/scalar.yml
   ```

3. Use the output in prompt-engineering workflows and codegen pipelines.

## Notes
- If you do not have Scalar installed, you can still use the OpenAPI metadata produced by each service (`/openapi/v1`) as a fallback.
- The key requirement is that server code and generated clients share a single, machine-readable contract source.
