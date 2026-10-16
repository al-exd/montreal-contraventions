# Changelog

All notable changes are documented here. Format based on [Keep a Changelog](https://keepachangelog.com/),
versioning follows [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Added

- Design documentation (vision, domain, architecture, testing, CI/CD, roadmap, ADRs 0001-0008).

### Added (increment 1)

- Backend skeleton: solution, `SharedKernel`, `Registry` and `Ingestion` module projects, `MontrealFoodViolations.Api` host with `/health`.
- Architecture tests enforcing project-reference rules; `ci.yml` (format, build with warnings as errors, tests with coverage, vulnerable packages).
- Removed the unmodified ASP.NET Core MVC template.

### Added (increment 2)

- Angular frontend skeleton with dev proxy to the API, app shell showing the dataset attribution, Vitest unit tests and a Playwright smoke test.
- CI: frontend job (unit tests, build, e2e), Gitleaks secret scan, Dependabot for NuGet, npm and GitHub Actions; CI also runs on `dev`.

### Added (increment 3)

- First deployment live on Oracle Always Free (`VM.Standard.E2.1.Micro`, A1 capacity unavailable in `ca-montreal-1`), HTTPS via Caddy and DuckDNS.
- Deployment: API and web (Caddy) Dockerfiles, `infra/compose.yml`, `infra/cloud-init.yml`, restricted `infra/deploy.sh` with tests, `cd.yml` (multi-arch images to GHCR, SSH deploy, smoke test, rollback), Oracle runbook and ADR-0011.

- Rollback drill passed (2026-10-16): a deployment with `/health` answering 502 failed its smoke test, the `Roll back` step restored the previous images, `/health` returned 200 and the job ended red.

### Fixed

- Page title is now MontrealFoodViolations (was the Angular scaffold default).

### Changed

- Roadmap revised and renumbered: walking-skeleton deployment at increment 3, backups at increment 13 before the feature checkpoint, single Identity setup (ADRs 0009 and 0010).
