# Roadmap

Status: Draft · Last updated: 2026-10-15 · Related: [vision](00-vision.md), [testing strategy](06-testing-strategy.md), [ADR-0009](adr/0009-single-identity-seeded-admin.md), [ADR-0010](adr/0010-early-deployment-and-backups.md)

## Table of contents

- [How increments work](#how-increments-work)
- [Increments](#increments)
- [Renumbering from the first draft](#renumbering-from-the-first-draft)

## How increments work

Each increment is a vertical slice delivered test-first (red, green, refactor) as its own set of commits or pull request,
with CI green before the next starts. Increments from 8 on ship an API slice and its Angular slice where a UI exists. A tag
`v0.x` marks each milestone and `CHANGELOG.md` is updated.

## Increments

### 1. Backend skeleton and CI
Solution, module projects, architecture tests, `/health`, CI workflow.
Acceptance: `dotnet test` runs in CI; the architecture test enforces the dependency rules; `/health` returns 200.

### 2. Frontend skeleton
Angular skeleton with proxy, Playwright, Gitleaks, Dependabot, dataset licence attribution in the app shell.
Acceptance: `npm test` and the e2e smoke test run in CI; the attribution is visible on every page.

### 3. Walking-skeleton deployment
Oracle runbook executed, `cd.yml`, HTTPS, `/health` smoke test, rollback to the previous tag.
Acceptance: a push to `master` deploys `/health` to a public HTTPS URL; a failed smoke test rolls back.

### 4. Domain core
Value objects (`BusinessId`, `ProsecutionId`, `Money`, `DateRange`), `Establishment` and `Violation` with invariants and revisions.
Acceptance: negative fine rejected; judgment before offence rejected; revision numbers strictly increase.

### 5. CSV anti-corruption layer
Parse the French CSV into domain records, reject bad rows individually.
Acceptance: BOM, accents, decimal comma, empty and invalid fields are handled; rejected rows reported with reasons.

### 6. Ingestion use case
Resilient HTTP feed, idempotent import, new-violation detection, outbox event, on in-memory fakes.
Acceptance: importing the same file twice creates no revisions; an unseen `ProsecutionId` yields exactly one `NewViolationsDetected`.

### 7. Persistence
EF Core mappings and migrations per module (own table prefix and history table).
Acceptance: repositories pass integration tests against real SQLite; no cross-module foreign keys; the idempotency test of increment 6 passes against SQLite; the revision and its outbox event are written atomically.

### 8. Search
Filters by name, operator, street, borough, category, status, date range, with paging; Angular search page.
Acceptance: results match filters; paging stable.

### 9. Date-range query
Dedicated `GET /api/v1/violations?from=&to=` and the quick-search widget counting violations per establishment (reuses the date filter of increment 8).
Acceptance: ISO 8601 dates validated; invalid range returns ProblemDetails; counts match the underlying violations.

### 10. Establishment detail
Autocomplete (decided here, replacing the dropdown option) and per-establishment violations.
Acceptance: all violations for the selected establishment are shown with full details.

### 11. Ranking
`/api/v1/establishments/violation-counts` in JSON, XML and CSV (UTF-8), sorted descending.
Acceptance: content negotiation works; the three formats carry identical data.

### 12. Scheduled sync
`BackgroundService` runs the import on a schedule (checked daily; the dataset changes monthly).
Acceptance: schedule driven by `TimeProvider` in tests; failures logged and retried.

### 13. Backups and restore
Daily `sqlite3 .backup` to Object Storage with retention (30 daily, 12 monthly), including the Data Protection key ring.
Acceptance: a restore from a backup onto a clean VM is verified once and documented in the runbook.

### Feature checkpoint
After increment 13, deploy increments 4 to 13 to Oracle (CD, HTTPS, health smoke test). Everything after this is deployed incrementally.

### 14. Early notifications
Digest to the configured recipient and Bluesky posts for new violators behind `ISocialPublisher`.
Acceptance: every message and post carries the TEST label when `EnvironmentLabel` is set and none when it is empty; the fake publisher receives one post per new establishment.

### 15. Admin curation and history
Identity built once with roles, the admin seeded from `ADMIN_PASSWORD_HASH` ([ADR-0009](adr/0009-single-identity-seeded-admin.md)); edit, delete, restore as new revisions; history view with diff.
Acceptance: an admin revision survives a later import; the city's data stays in history; nothing is physically removed; deleted entities are hidden from search.

### 16. Inspection requests
Create (validated) and withdraw; complaint page.
Acceptance: invalid payload returns field errors; withdraw is idempotent.

### 17. Subscribers
Registration and login on the existing Identity, watchlist, avatar upload (jpg/png, size and magic-byte check).
Acceptance: passwords hashed; invalid image rejected; watchlist editable.

### 18. Subscriber alerts
Per-subscriber alerts with signed unsubscribe link, confirmation page and API.
Acceptance: TEST label rules as in increment 14; at most one alert per violation per subscriber; allow-list and rate caps enforced.

### 19. Hardening
Security headers and rate limiting.
Acceptance: the headers are present on every response; excess requests receive 429 with ProblemDetails.

### 20. Observability
Structured logging, OpenTelemetry, health checks.
Acceptance: each request and each import carries a correlation id in the logs; health checks cover the database and the last successful sync.

### 21. Statistics dashboard (stretch)
Fines by borough, category and year; top offenders.
Acceptance: figures match a direct count on the same data; empty ranges render without error.

### 22. Internationalization and accessibility (stretch)
English and French UI and emails with a language switch; accessibility pass.
Acceptance: no hard-coded UI strings remain; automated accessibility checks report no serious violations on the main pages.

## Renumbering from the first draft

| First draft | Now |
|---|---|
| 1 | 1, 2 and 3 |
| 2, 3, 4, 5 | 4, 5, 6, 7 |
| 6, 7, 8, 9, 10 | 8, 9, 10, 11, 12 |
| 11 | 15 |
| 12, 13 | 16, 17 |
| 14 | 14 (digest) and 18 (subscriber alerts) |
| 15 | 14 (Bluesky) |
| 16 | 21 |
| 17 | 19 (hardening) and 22 |
| 18 | 13 (backups) and 20 |
