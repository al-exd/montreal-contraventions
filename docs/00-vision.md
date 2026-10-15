# Vision

Status: Draft · Last updated: 2026-10-15 · Related: [ADR-0001](adr/0001-modular-monolith-ddd-hexagonal.md)

## Table of contents

- [Problem](#problem)
- [Goals](#goals)
- [Users](#users)
- [Scope](#scope)
- [Non-goals](#non-goals)
- [Constraints](#constraints)
- [Success criteria](#success-criteria)

## Problem

The City of Montréal publishes food-establishment convictions under the *Loi sur les produits alimentaires*
as open data ([Inspection des aliments – contrevenants](https://donnees.montreal.ca/dataset/inspection-aliments-contrevenants)):
a ~4 MB CSV (`violations.csv`), updated monthly, covering 2011 onward. The raw file is hard to search, has no
history of corrections, and offers no way to be alerted when a place you care about is convicted.

## Goals

1. Make the dataset searchable and understandable (search, date ranges, rankings, per-establishment detail).
2. Keep a faithful, versioned archive of the data, including administrator corrections, without ever losing the city's original.
3. Notify interested people (watchlist subscribers, a configured recipient, a social account) when new violations appear.
4. Be a showcase of modern .NET and Angular engineering for a portfolio and CV: DDD, TDD, CI/CD, zero-cost cloud deployment.

## Users

| Actor | Needs |
|---|---|
| Visitor (anonymous) | Search violations, view an establishment, see rankings, file an inspection request |
| Subscriber (authenticated) | Maintain a watchlist, receive alert emails, unsubscribe, manage a profile picture |
| Administrator | Correct or delete records while preserving history; restore previous revisions |
| System (scheduler) | Import the dataset, detect new violations, trigger notifications |
| Social account (Bluesky) | Receive automatic posts of new violators' names |

## Scope

Prioritized with MoSCoW. Increment numbers refer to the [roadmap](roadmap.md).

| Priority | Capability | Increment |
|---|---|---|
| Must | CSV ingestion with idempotent import and new-violation detection | 5, 6, 7 |
| Must | Search, date-range query, per-establishment detail | 8, 9, 10 |
| Must | Violation counts ranking in JSON, XML and CSV | 11 |
| Must | Scheduled synchronisation | 12 |
| Must | Early deployment, backups and a verified restore | 3, 13 |
| Must | Admin curation with append-only revisions (archive) | 15 |
| Should | Inspection requests (citizen complaints) | 16 |
| Should | Subscribers, watchlist, avatar upload | 17 |
| Should | Email notifications, unsubscribe flow | 14, 18 |
| Could | Bluesky publication | 14 |
| Could | Statistics dashboard | 21 |
| Could | EN/FR internationalization, accessibility pass | 22 |
| Must | Hardening (security headers, rate limiting) | 19 |
| Should | Observability (logging, OpenTelemetry, health checks) | 20 |

## Non-goals

- Replacing or being an official City of Montréal service.
- Geolocation or map display (the dataset has no coordinates in the feed we ingest).
- Multi-tenant or horizontally scaled operation (single deployable, single SQLite file).
- Paid cloud services of any kind.

## Constraints

- Oracle Cloud Always Free tier only, no billing ([guardrails](07-cicd-and-deployment.md#zero-cost-guardrails)).
- SQLite persistence, .NET 10 Web API, Angular SPA.
- English documentation; UI available in English and French.
- Public open data: attribution and the dataset's license must be respected (check the "Licence d'utilisation" page).

## Success criteria

- A public HTTPS URL serves the app, deployed automatically from `master` by GitHub Actions.
- All increments delivered test-first with CI green; architecture tests enforce module boundaries.
- An admin edit survives a re-import and the earlier revision remains viewable.
- Month-end invoice from Oracle: zero.
