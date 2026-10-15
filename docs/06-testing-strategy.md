# Testing strategy

Status: Draft · Last updated: 2026-10-14 · Related: [architecture](05-architecture.md), [roadmap](roadmap.md)

## Table of contents

- [Approach](#approach)
- [Test pyramid](#test-pyramid)
- [TDD loop](#tdd-loop)
- [What is faked](#what-is-faked)
- [Real-email smoke test](#real-email-smoke-test)
- [Naming and structure](#naming-and-structure)
- [Quality gates](#quality-gates)

## Approach

Every increment starts with a failing test (red), then the smallest change to pass (green), then refactoring with the
suite as a safety net. Tests describe behaviour in the [ubiquitous language](02-ubiquitous-language.md).

## Test pyramid

| Level | Scope | Tools | Share |
|---|---|---|---|
| Unit: Domain | Aggregates, value objects, invariants, revision rules; no I/O | xUnit, FluentAssertions | Largest |
| Unit: Application | Use cases against fake ports | xUnit, NSubstitute | Large |
| Integration: Infrastructure | EF Core against real SQLite (temp file), CSV parser against fixtures, MailKit decorator | xUnit, SQLite | Medium |
| API | Controllers through the full pipeline | `WebApplicationFactory` | Medium |
| Architecture | Dependency and boundary rules | NetArchTest | Small, always on |
| Frontend unit | Components and services | Vitest/Jest, Angular Testing Library | Medium |
| End to end | Critical journeys against the docker compose stack | Playwright | Few |

## TDD loop

1. Write one failing test that expresses the next behaviour.
2. Run it and watch it fail for the right reason.
3. Write the minimum code to pass.
4. Refactor; keep all tests green.
5. Commit (small, descriptive); CI runs on every push.

## What is faked

| Dependency | Test double |
|---|---|
| City CSV feed | Fixture file (`violations.sample.csv`) via a stub `HttpMessageHandler` or fixture `IViolationFeed` |
| Clock | `FakeTimeProvider` |
| Email | Fake `IEmailSender` recording messages; the TEST-label decorator is tested against it |
| Bluesky | Fake `ISocialPublisher` |
| Database | Not faked: real SQLite (in memory or temp file) |

Fixtures include edge cases: BOM, accents, decimal comma, empty fields, bad dates, duplicate `id_poursuite`, negative fine.

## Real-email smoke test

Automated tests never send real mail. After a deployment, a manually triggered workflow (`workflow_dispatch`,
environment `production`) or an admin-only API call sends one real message to the owner. It must carry the visible
TEST label ([ADR-0005](adr/0005-gmail-smtp-and-test-label.md)).

## Naming and structure

- Test project per module layer: `Registry.Domain.Tests`, `Registry.Application.Tests`, and so on.
- Method names: `Method_Scenario_ExpectedResult`, or `Given_When_Then` for use cases.
- Arrange/Act/Assert with blank lines; one logical assertion per test.
- Builders and Bogus for test data; no shared mutable state between tests.

## Quality gates

- CI fails on any test failure, warning (warnings are errors), or formatting drift.
- Coverage threshold on Domain and Application (target 80%, raised over time).
- Architecture tests are mandatory and block merges.
- Optional: mutation testing (Stryker.NET) on Domain, run on demand.
