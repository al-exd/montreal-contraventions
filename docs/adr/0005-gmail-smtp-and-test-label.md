# 0005. Gmail SMTP and visible TEST label

Status: Accepted · Date: 2026-10-14

## Context and problem statement

The system sends real emails (alerts, digests). While it is a demo, every message must be clearly identifiable as a test, and a personal mailbox must be protected from runaway sending.

## Considered options

1. Fake or no-op sender in non-test environments.
2. Real SMTP with a transactional provider free tier.
3. Real SMTP through the owner's existing Gmail account with an app password (chosen by the owner).

## Decision outcome

Option 3 using `smtp.gmail.com:587` (STARTTLS) via MailKit. The owner's existing personal Gmail account and app password are used; the password is supplied at deploy time and never stored in the repository or docs.

A single decorator around `IEmailSender` applies the label when `EmailOptions.EnvironmentLabel` is set: subject prefix `[TEST] `, header `X-Environment: TEST`, and a visible banner in HTML and plain text. Safety rails: recipient allow-list, per-run and daily rate caps. Oracle blocks outbound port 25, so only 587 or 465 are used. SMTP host, port and user are options, so switching provider is configuration.

## Consequences

- Good: real end-to-end emails at no cost; labelling logic in one tested place.
- Bad: messages appear in the owner's Sent folder; Gmail limits (about 500 per day) and From rewriting; personal credential must be protected (see [ADR-0008](0008-secrets-management.md)).
