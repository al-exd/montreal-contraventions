# AGENTS.md

Guidance for AI coding agents working in this repository. Humans: see [README.md](README.md) and [docs/](docs/README.md).

## Principles

- Simplicity and convention over configuration; build incrementally from a working base.
- Follow the design in `docs/`; when you change a decision, add a new ADR instead of editing an old one.
- Test first (red, green, refactor). No production code without a failing test that justifies it.
- Keep the language of the [ubiquitous language](docs/02-ubiquitous-language.md): English terms in code, French only in the CSV anti-corruption layer and translations.

## Architecture rules

- Modular monolith: one module per bounded context, hexagonal inside each (Domain, Application, Infrastructure, Contracts).
- Modules reference each other only through `*.Contracts`. Domain has no dependencies. Architecture tests enforce this.
- One EF Core `DbContext` per module; no cross-module joins or foreign keys.
- Data is never overwritten or physically deleted: append revisions ([ADR-0006](docs/adr/0006-append-only-revisions.md)).

## Commands (once increment 1 is done)

- Build: `dotnet build -warnaserror`
- Test: `dotnet test`
- Format check: `dotnet format --verify-no-changes`
- Frontend: `npm ci && npm test && npm run build` in `frontend/`

## Safety

- Never commit secrets; use `dotnet user-secrets` locally and GitHub Environment secrets in CI/CD ([ADR-0008](docs/adr/0008-secrets-management.md)).
- All outgoing email and social posts carry the TEST label while `EnvironmentLabel` is set ([ADR-0005](docs/adr/0005-gmail-smtp-and-test-label.md)).
- Stay inside Oracle Always Free limits; never suggest upgrading the account ([guardrails](docs/07-cicd-and-deployment.md#zero-cost-guardrails)).
