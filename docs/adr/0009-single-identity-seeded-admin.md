# 0009. One Identity setup, admin seeded from a secret

Status: Accepted · Date: 2026-10-15

## Context and problem statement

Administrators curate data (increment 15) and subscribers register later (increment 17). The secrets policy already provides a single `ADMIN_PASSWORD_HASH`. Building one mechanism for the admin and another for subscribers would duplicate authentication.

## Considered options

1. ASP.NET Core Identity built once with roles; the admin account is seeded from `ADMIN_PASSWORD_HASH`; subscribers are added later (chosen).
2. A single configured admin account without Identity, and Identity only for subscribers.

## Decision outcome

Option 1. The `Identity` module is created in increment 15 with an `Admin` role. The admin is seeded at startup from the secret and never stored in the repository ([ADR-0008](0008-secrets-management.md)). Increment 17 adds subscriber registration on the same module, with no second authentication path.

## Consequences

- Good: one login flow, one place for password hashing, lockout and cookies.
- Good: the admin seed keeps working with the existing secret handling.
- Bad: Identity arrives in the admin increment, a little heavier than a configured account.
