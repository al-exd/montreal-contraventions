# MontrealFoodViolations — Documentation index

Status: Draft · Last updated: 2026-10-15

Design documentation, written before any production code (DDD first, then TDD increments).

## Contents

1. [Vision](00-vision.md): problem, users, scope, priorities
2. [Domain discovery](01-domain-discovery.md): event storming result
3. [Ubiquitous language](02-ubiquitous-language.md): glossary, EN to FR dataset mapping
4. [Bounded contexts](03-bounded-contexts.md): context map
5. [Domain model](04-domain-model.md): aggregates, value objects, revisions, events
6. [Architecture](05-architecture.md): modular monolith, hexagonal modules, C4 diagrams
7. [Testing strategy](06-testing-strategy.md)
8. [CI/CD and deployment](07-cicd-and-deployment.md): pipelines, Oracle runbook, secrets, zero-cost guardrails
9. [Roadmap](roadmap.md): TDD increments with acceptance criteria
10. [Architecture decision records](adr/README.md)
11. [Oracle runbook](08-oracle-runbook.md): console steps for the first deployment

## Conventions

- Markdown with a table of contents, status/date header and language-tagged code blocks.
- Diagrams are [Mermaid](https://mermaid.js.org/) blocks, rendered natively by GitHub.
- ADRs follow the [MADR](https://adr.github.io/madr/) template, are numbered and never rewritten (superseded via status).
- `docs/cv-notes.md` is written last, after the first deployment.
