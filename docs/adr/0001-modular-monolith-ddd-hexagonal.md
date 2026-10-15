# 0001. Modular monolith by bounded context, hexagonal inside

Status: Accepted · Date: 2026-10-14

## Context and problem statement

The system has distinct domain areas (registry, ingestion, watchlist, requests, identity) but a single small team, a single free VM and a single deployable. We want strong boundaries and DDD without distributed-system cost.

## Decision drivers

- Showcase DDD strategic and tactical design in .NET.
- Zero infrastructure cost; one deployable.
- Keep modules extractable later.

## Considered options

1. Layered monolith (one Domain/Application/Infrastructure for everything).
2. Modular monolith, one module per bounded context, hexagonal inside each (chosen).
3. Microservices.

## Decision outcome

Option 2. Modules communicate only through `*.Contracts` (facades and integration events). Inside a module: Domain has no dependencies, Application depends on Domain, Infrastructure on both. Architecture tests (NetArchTest) enforce this. A module may begin as one project with folders and `internal` visibility and be split into assemblies when justified.

## Consequences

- Good: clear boundaries, testable domain, cheap to run, evolvable.
- Bad: more projects and ceremony than a layered app; boundaries rely on discipline backed by tests.
