# 0002. SQLite with one EF Core DbContext per module

Status: Accepted · Date: 2026-10-14

## Context and problem statement

We need persistence that costs nothing, is easy to back up, and respects module data ownership.

## Considered options

1. SQLite, single shared DbContext.
2. SQLite, one DbContext per module, same file (chosen).
3. PostgreSQL container or Oracle Autonomous DB.

## Decision outcome

Option 2. All DbContexts target the same SQLite file; each module has its own table prefix (`registry_*`, `ingestion_*`, ...) and its own migrations history table. No cross-module joins or foreign keys; modules exchange data through contracts and events. Cross-module consistency uses the outbox pattern rather than shared transactions.

## Consequences

- Good: enforced data ownership, extractable modules, trivial backup (one file), zero cost.
- Bad: several migration sets; no cross-module transactions; SQLite single-writer limits (acceptable at this scale; use WAL mode).
