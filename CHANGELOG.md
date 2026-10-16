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

### Changed

- Roadmap revised and renumbered: walking-skeleton deployment at increment 3, backups at increment 13 before the feature checkpoint, single Identity setup (ADRs 0009 and 0010).
