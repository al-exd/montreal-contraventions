# 0003. Docker on Oracle Always Free with Caddy

Status: Accepted · Date: 2026-10-14

## Context and problem statement

Deploy to the cloud at zero cost, automatically from GitHub, with HTTPS.

## Considered options

1. Docker Compose on one Always Free compute VM, Caddy for TLS (chosen).
2. OKE (Kubernetes).
3. Plain systemd publish with scp.
4. Ansible or Terraform to provision the VM.

## Decision outcome

Option 1. Ampere A1 (2 OCPU, 12 GB planned) preferred; fallback `VM.Standard.E2.1.Micro` with swap and a `linux/amd64` image if A1 capacity is unavailable to a non-upgraded account. Caddy serves the built SPA and proxies `/api/*`, with automatic Let's Encrypt certificates on a free DNS name. Infrastructure is created once by hand in the OCI console and bootstrapped by `cloud-init`; no Ansible or Terraform (disproportionate for one VM).

## Consequences

- Good: simple, free, same-origin (no CORS), fast rollback by image tag.
- Bad: manual one-time setup; possible A1 capacity wait; Oracle may reclaim idle instances (mitigated by health pings and rebuildable VM from backups).
- Hard rule: stay inside Always Free; never upgrade the account. See [guardrails](../07-cicd-and-deployment.md#zero-cost-guardrails).
