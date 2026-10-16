# MontrealFoodViolations

A web app that ingests the City of Montréal open dataset
[Inspection des aliments – contrevenants](https://donnees.montreal.ca/dataset/inspection-aliments-contrevenants),
keeps a versioned archive of it, and offers search, rankings, watchlist alerts and administrator curation.

Built as a .NET and Angular portfolio project: domain-driven design, test-driven development, CI/CD to the
Oracle Cloud Always Free tier at zero cost.

Status: **increment 1 of the [roadmap](docs/roadmap.md) done** (backend skeleton and CI). Documentation is written first; implementation follows in test-first increments.

## Table of contents

- [Documentation](#documentation)
- [Planned stack](#planned-stack)
- [Getting started](#getting-started)
- [Data attribution](#data-attribution)

## Documentation

Start at [docs/README.md](docs/README.md): vision, domain discovery, ubiquitous language, bounded contexts,
domain model, architecture, testing strategy, CI/CD, roadmap and architecture decision records.

## Planned stack

- .NET 10, ASP.NET Core Web API, EF Core + SQLite, modular monolith with hexagonal modules
- Angular SPA (standalone components, signals), English and French
- xUnit, NetArchTest, Playwright
- GitHub Actions, GHCR, Docker Compose + Caddy on an Oracle Always Free VM

## Getting started

Requires the .NET 10 SDK. The backend skeleton is in place (frontend and deployment come in increments 2 and 3):

```bash
dotnet build -warnaserror
dotnet test
dotnet run --project src/Host/Api   # then GET /health
```

## Data attribution

Data: City of Montréal, Division de l'inspection des aliments, published on the Montréal open data portal.
Check the dataset's license page before reuse.
