# 0010. Early deployment and backups before irreplaceable data

Status: Accepted · Date: 2026-10-15 · Refines [ADR-0003](0003-docker-on-oracle-always-free-with-caddy.md)

## Context and problem statement

The first roadmap deployed only after ten increments and placed backups last, although the backup is the archive of record. Oracle unknowns (Ampere A1 capacity, idle reclamation, DNS, the SSH deploy key) would surface late, and admin edits could go live without a backup.

## Considered options

1. Walking-skeleton deployment right after the skeletons, backups before the feature checkpoint (chosen).
2. Keep one deployment after the first ten increments, with backups last.

## Decision outcome

Option 1. Increment 3 deploys `/health` to Oracle through CD. Increment 13 adds backups and one verified restore before the feature checkpoint, so no real data is deployed without a backup. ADR-0003 is unchanged; only the rollout order changes. Increments were renumbered; the table in the [roadmap](../roadmap.md#renumbering-from-the-first-draft) maps old to new. Increment numbers quoted in ADR-0004 (15) and ADR-0007 (17) refer to the first draft, which are now 14 and 22.

## Consequences

- Good: deployment risks appear while changes are cheap; backups exist before irreplaceable data.
- Good: the feature checkpoint becomes a routine deploy.
- Bad: the Oracle setup effort moves to the beginning of the project.
