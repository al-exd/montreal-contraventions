# 0006. Append-only revisions and import vs admin precedence

Status: Accepted · Date: 2026-10-14

## Context and problem statement

The monthly import re-downloads the whole CSV, but administrators may correct or delete records. Edits and deletions must survive imports, and the owner wants archives: every version of the data kept.

## Considered options

1. Mutable records plus override and tombstone tables (local data wins).
2. Append-only revisions for every change, with a computed current view (chosen).
3. Event sourcing.

## Decision outcome

Option 2. Each establishment and violation is a series of immutable `Revision` rows (number, validity time, source `Import(batchId)` or `Admin(userId, reason)`, data snapshot, content hash, `IsDeleted`). An import appends a revision only if the content hash differs from the latest import revision. The current view is the latest admin revision if one exists, else the latest import revision; deleted entities are hidden from normal queries. Restore and "accept city version" append new admin revisions. Revisions are never purged; the daily SQLite backup doubles as the archive.

The current view is either a SQL view or a flag maintained in the same transaction; EF Core global query filters expose only current, non-deleted rows, and history queries use `IgnoreQueryFilters`. New violations are detected as "no prior revision for this ProsecutionId".

## Consequences

- Good: full audit and archive, safe admin edits, simple importer, natural new-violation detection.
- Bad: more rows and more complex queries than mutable tables; storage is negligible at this data size (about 4 MB CSV).
