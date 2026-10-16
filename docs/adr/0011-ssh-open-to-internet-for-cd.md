# 0011. SSH reachable from the internet for continuous deployment

Status: Accepted · Date: 2026-10-15 · Refines [ADR-0003](0003-docker-on-oracle-always-free-with-caddy.md) and [ADR-0008](0008-secrets-management.md)

## Context and problem statement

The runbook restricted SSH (port 22) to the owner's IP. GitHub-hosted runners have no fixed address, so `cd.yml` could never reach the VM. Alternatives cost money or add moving parts.

## Considered options

1. Open port 22 to the internet; protect it with key-only authentication and a forced-command deploy key (chosen).
2. Allow the published GitHub Actions IP ranges (hundreds of CIDR blocks that change; the security list limit is 200 rules).
3. Self-hosted runner on the VM (always-on agent with repository access on the host it deploys to).
4. Pull-based deployment (a timer on the VM polls for a new tag), no inbound SSH from CI.

## Decision outcome

Option 1. Password authentication is disabled by default on the Ubuntu image. The CD key is ed25519, belongs to the non-root `deploy` user and is restricted with `restrict,command="/opt/app/deploy.sh"`: it can only deploy or roll back, with no shell, tty or forwarding. The host key is pinned in `DEPLOY_KNOWN_HOSTS`. The owner's admin access uses a different key on the `ubuntu` user.

Option 4 stays the fallback if scanner noise or abuse becomes a problem.

## Consequences

- Good: simple, free, no extra agent on the VM.
- Bad: port 22 is exposed to scanners (log noise; mitigated by key-only login and unattended upgrades). A leaked deploy key can deploy arbitrary image tags from the project's registry, not run commands.
- Neutral: the `deploy` user is in the `docker` group, which is root-equivalent on the host; the forced command is what limits it.
