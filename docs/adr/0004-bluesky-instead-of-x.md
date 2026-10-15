# 0004. Bluesky instead of X

Status: Accepted · Date: 2026-10-14

## Context and problem statement

Publishing new violators' names to a social account is a nice-to-have. The X (Twitter) API is no longer freely available for posting at useful volume.

## Considered options

1. X API (paid tiers).
2. Bluesky via the AT Protocol (free, app passwords) (chosen).
3. Skip social publishing.

## Decision outcome

Option 2, behind the `ISocialPublisher` port so it can be swapped or disabled. Posts carry the `[TEST]` label while the environment label is set and are sent only to the owner's account. Implemented late (increment 15), optional.

## Consequences

- Good: free, simple HTTP API, port keeps the domain independent.
- Bad: smaller audience; one more credential (`BLUESKY_*` app password) to manage.
