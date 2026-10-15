# Ubiquitous language

Status: Draft · Last updated: 2026-10-14 · Related: [domain model](04-domain-model.md)

## Table of contents

- [Dataset field mapping](#dataset-field-mapping)
- [Glossary](#glossary)
- [Language rules](#language-rules)

## Dataset field mapping

The source dataset is in French. Code, docs and API use the English term; the French field name appears only in the
anti-corruption layer of the Ingestion module.

| CSV field (FR) | Domain term (EN) | Type | Notes |
|---|---|---|---|
| `id_poursuite` | `ProsecutionId` | integer | Identity of a Violation |
| `business_id` | `BusinessId` | integer | Identity of an Establishment |
| `date` | `OffenceDate` | date | Date of the violation |
| `date_jugement` | `JudgmentDate` | date | Must be on or after the offence date |
| `description` | `Description` | text | Free text, may contain accents and line breaks |
| `etablissement` | `EstablishmentName` | text | |
| `montant` | `Fine` (`Money`) | decimal CAD | Never negative |
| `proprietaire` | `Operator` | text | Person or company operating the establishment |
| `ville` | `Borough` | text | Borough or municipality |
| `Statut` | `OperationalStatus` | enum | `Ouvert`=Open, `Fermé`=ClosedPermanently, `Fermé changement d'exploitant`=ClosedOperatorChange, `Sous inspection fédérale`=FederalInspection |
| `date_statut` | `StatusDate` | date | Date of latest status |
| `categorie` | `Category` | text | Establishment category |

## Glossary

| Term | Meaning |
|---|---|
| Establishment | A food business that has at least one conviction; aggregate root of the Registry context |
| Violation | One conviction (prosecution) against an establishment |
| Revision | An immutable snapshot of an establishment or violation at a point in time, from an import or an admin action |
| Current view | The revision shown to users: latest admin revision, else latest import revision, unless deleted |
| Import batch | One execution of the synchronisation, identified by id and timestamp |
| New violation | A violation whose `ProsecutionId` has no prior revision |
| Watchlist | The set of establishments a subscriber follows |
| Inspection request | A citizen's complaint asking for an inspection of an establishment |
| Curation | Admin correction or deletion of a record |
| Archive | The full revision history, kept forever |
| Test label | The `[TEST]` marker added to every outgoing email and social post in non-production-release environments |

## Language rules

1. One term per concept; no synonyms (say *violation*, not *infraction*, *contravention* or *offence* in code).
2. Names inside a bounded context follow that context's model; do not leak another context's types.
3. French text appears only in the data, the French UI translations and the ACL mapping.
