# 0008. Secrets management

Status: Accepted · Date: 2026-10-14

## Context and problem statement

The system uses an SMTP app password, a Bluesky app password, an admin password hash, deploy credentials and DNS tokens. None may leak, and the setup must stay free.

## Decision outcome

| Where | How |
|---|---|
| Source control | No secrets committed; `.gitignore` for `.env*`, `*.pfx`, `appsettings.*.local.json`; Gitleaks as pre-commit hook and CI job; GitHub secret scanning and push protection |
| Local dev | `dotnet user-secrets`; Mailpit for SMTP; no-op or fake adapters by default |
| Tests and CI | No real credentials; fakes for email and social ports |
| GitHub | Environment `production` holds all deploy and app secrets, restricted to `master`, required reviewer for tag deploys; minimal workflow `permissions`; actions pinned by SHA; Dependabot |
| CD to VM | Dedicated ed25519 deploy key for a non-root `deploy` user, restricted with `restrict,command="..."`, host key pinned; no OCI API key in CI |
| VM runtime | `/opt/app/.env` (owner `deploy`, mode 600) written by CD from environment secrets |
| App | Options pattern with `ValidateOnStart` (fail fast); Serilog redaction; passwords hashed with ASP.NET Core Identity; unsubscribe links use signed, expiring tokens (Data Protection API, key ring on a volume) |
| Rotation | Change the secret in the GitHub Environment and redeploy; rotate the deploy key and pull token every 90 days |

ed25519 is preferred over RSA: short keys, fast, fixed strong parameters, default in current OpenSSH. RSA-4096 only if a tool rejects ed25519.

## Consequences

- Good: no long-lived secrets in the repository; least privilege for CD.
- Bad: secrets exist on the VM disk (mitigated by file permissions, single-purpose VM, and rotation).
