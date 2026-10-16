# Oracle runbook: first deployment

Status: Draft · Last updated: 2026-10-15 · Related: [CI/CD and deployment](07-cicd-and-deployment.md), [ADR-0003](adr/0003-docker-on-oracle-always-free-with-caddy.md), [ADR-0011](adr/0011-ssh-open-to-internet-for-cd.md)

One-time setup of the Always Free VM, then the first automatic deployment. Console menu names change from time to time; follow the intent if a label differs.

## Table of contents

- [Before you start](#before-you-start)
- [1. Keys on your machine](#1-keys-on-your-machine)
- [2. Account, region and budget](#2-account-region-and-budget)
- [3. Network](#3-network)
- [4. Instance](#4-instance)
- [5. Check the VM](#5-check-the-vm)
- [6. DNS name](#6-dns-name)
- [7. GitHub configuration](#7-github-configuration)
- [8. First deployment](#8-first-deployment)
- [9. Verify and test the rollback](#9-verify-and-test-the-rollback)
- [Troubleshooting](#troubleshooting)

## Before you start

- Everything below stays inside Always Free. **Never click "Upgrade" or "Pay As You Go"** ([guardrails](07-cicd-and-deployment.md#zero-cost-guardrails)).
- `infra/deploy.sh` is downloaded by the VM from the `master` branch, and `cd.yml` only runs from `master`. Merge `dev` into `master` before step 4 (or at least before step 8) so both exist there.

## 1. Keys on your machine

Two separate ed25519 keys: one for you (admin), one for GitHub Actions (deploy).

```bash
ssh-keygen -t ed25519 -f ~/.ssh/mfv_admin -C "mfv admin"
ssh-keygen -t ed25519 -f ~/.ssh/mfv_deploy -N "" -C "mfv github deploy"
```

The deploy key has no passphrase because CI cannot type one. Never commit either private key.

Prepare the cloud-init script: copy `infra/cloud-init.yml` to a scratch file outside the repository and replace the word `DEPLOY_PUBLIC_KEY` with the whole content of `~/.ssh/mfv_deploy.pub` (one line, starting with `ssh-ed25519`).

## 2. Account, region and budget

1. Sign up at <https://www.oracle.com/cloud/free/>. Choose the **home region** carefully: it cannot be changed, and Always Free resources live there. Prefer a region where Ampere A1 capacity is likely (try a less popular one).
2. Console → **Billing & Cost Management → Budgets → Create budget**: scope "Tenancy" (compartment: root), amount **1 USD** per month.
3. Add two alert rules on that budget: **Actual spend 1%** and **Forecast spend 100%**, recipient your email.
4. Console → **Billing & Cost Management → Cost Analysis**: bookmark it; check it monthly.

## 3. Network

Do not use the "VCN wizard with internet connectivity": it creates a NAT gateway, which the guardrails exclude.

1. **Networking → Virtual cloud networks → Create VCN**: name `mfv-vcn`, IPv4 CIDR `10.0.0.0/16`, no IPv6.
2. Open the VCN → **Gateways → Internet gateways → Create**: name `mfv-igw`.
3. VCN → **Routing → Default route table → Add route rule**: target type *Internet Gateway*, destination CIDR `0.0.0.0/0`, target `mfv-igw`.
4. VCN → **Subnets → Create subnet**: name `mfv-public`, regional, CIDR `10.0.0.0/24`, route table *Default*, **Public subnet**, security list *Default*.
5. VCN → **Security → Default security list → Add ingress rules** (source CIDR `0.0.0.0/0`, stateful):
   - TCP, destination port `80`
   - TCP, destination port `443`

   Keep the existing TCP 22 rule (open to the world; key-only login, see [ADR-0011](adr/0011-ssh-open-to-internet-for-cd.md)). Do not add UDP 443.

## 4. Instance

**Compute → Instances → Create instance**:

| Field | Value |
|---|---|
| Name | `mfv` |
| Image | **Canonical Ubuntu 24.04** (Change image → Ubuntu) |
| Shape | Change shape → **Ampere → VM.Standard.A1.Flex**, **2 OCPU, 12 GB** |
| Networking | existing VCN `mfv-vcn`, subnet `mfv-public`, **Assign a public IPv4 address** |
| SSH keys | **Paste public key**: contents of `~/.ssh/mfv_admin.pub` |
| Boot volume | Specify a custom size: **50 GB** (never more, free limit is 200 GB total across volumes) |
| Advanced options → Management | **Paste cloud-init script**: the edited copy from step 1 |

Check that the page shows an "Always Free-eligible" label, then **Create**.

- **"Out of capacity for shape VM.Standard.A1.Flex"**: try another availability domain, retry every few hours, or fall back to **VM.Standard.E2.1.Micro** (x86, 1 GB; the cloud-init adds swap and the published images are multi-arch). Do not upgrade the account to get capacity.
- Note the **public IP** once the instance is *Running*.

## 5. Check the VM

```bash
ssh -i ~/.ssh/mfv_admin ubuntu@<public-ip>
cloud-init status --wait        # expect: status: done
docker --version && docker compose version
ls -l /opt/app/deploy.sh        # owned by deploy, executable
sudo cat /home/deploy/.ssh/authorized_keys   # starts with: restrict,command="/opt/app/deploy.sh" ssh-ed25519 ...
ssh-keygen -lf /etc/ssh/ssh_host_ed25519_key.pub   # note the fingerprint
```

If `cloud-init status` reports an error, read `/var/log/cloud-init-output.log`.

`/opt/app/deploy.sh` is downloaded from the public `master` branch at boot. If the repository is private, or the download failed for any reason, the file is missing: install it from your machine (repository root), which works in every case:

```bash
scp -i ~/.ssh/mfv_admin infra/deploy.sh ubuntu@<public-ip>:/tmp/deploy.sh
ssh -i ~/.ssh/mfv_admin ubuntu@<public-ip> \
  'sudo install -d -o deploy -g deploy /opt/app && sudo install -o deploy -g deploy -m 755 /tmp/deploy.sh /opt/app/deploy.sh'
```

Then check that the restricted key reaches the script and nothing else: `ssh -i ~/.ssh/mfv_deploy deploy@<public-ip> rollback` must print `no .env.previous: nothing to roll back to`.

## 6. DNS name

Caddy needs a hostname to get a Let's Encrypt certificate. A free subdomain is enough; no token is needed because the certificate uses the HTTP challenge on port 80.

1. Sign in at <https://www.duckdns.org/>, add a subdomain (for example `mfv-demo`), and set its IP to the VM public IP.
2. Your site address is then `mfv-demo.duckdns.org`.

Fallback without any account: `<ip-with-dashes>.nip.io` (for example `203-0-113-10.nip.io`), but its shared domain can hit Let's Encrypt rate limits.

## 7. GitHub configuration

Repository **Settings → Environments → New environment** named `production`:

- **Deployment branches and tags**: selected branches: `master`; add the tag pattern `v*`.
- Optional: *Required reviewers* yourself (every deploy then waits for approval).

Environment **secrets**:

| Name | Value |
|---|---|
| `DEPLOY_SSH_KEY` | full content of `~/.ssh/mfv_deploy` (the private key, including the BEGIN/END lines) |
| `DEPLOY_HOST` | the DNS name from step 6 |
| `DEPLOY_KNOWN_HOSTS` | output of `ssh-keyscan -t ed25519 <dns-name>`; first check that its key matches the fingerprint noted in step 5 (`ssh-keygen -lf` on the scanned line) |

Environment **variable** (not secret): `SITE_ADDRESS` = the DNS name from step 6.

Also enable **Settings → Code security**: secret scanning and push protection.

## 8. First deployment

1. Merge `dev` into `master` (pull request). `ci` runs, then `cd` starts by itself (or run **Actions → cd → Run workflow** on `master`).
2. The `publish` job pushes `ghcr.io/<owner>/montreal-contraventions-api` and `-web` (amd64 and arm64).
3. **First time only**: GHCR packages start private. On GitHub go to your profile → **Packages**, open each of the two packages → **Package settings → Change visibility → Public**. Then re-run the failed `deploy` job. (The VM pulls without credentials; see [ADR-0008](adr/0008-secrets-management.md).)
4. The `deploy` job sends `compose.yml` and a generated `.env` over SSH, the VM pulls and starts the stack, and the smoke test calls `https://<site>/health`.

## 9. Verify and test the rollback

```bash
curl -i https://<site>/health          # 200, valid certificate
```

Open `https://<site>/` in a browser: the shell with the dataset attribution is shown.

### Rollback drill (increment 3 acceptance)

Prove once that a failed smoke test restores the previous version. Do not point the repository variable `SITE_ADDRESS` at a wrong name: the rollback's own smoke test reads the same variable and would fail even after a successful rollback.

Instead, break `/health` where the unit tests cannot see it, so CI stays green and CD runs:

1. On `dev`, in `infra/Caddyfile`, change `handle /health { reverse_proxy api:8080 }` to proxy to `api:9999` (nothing listens there, so `/health` answers 502). Commit, push, merge to `master`.
2. Watch **Actions → cd**: `publish` and `Deploy` succeed, `Smoke test` fails after its 60 s of retries, `Roll back` runs and the job ends red with the error "rolled back to the previous version".
3. Check `curl -i https://<site>/health` is **200** again and `/opt/app/.env` on the VM holds the previous image tag. After a rollback `.env.previous` still holds that last good tag (a rollback does not overwrite it), so a second rollback is a no-op.
4. Revert the commit and merge; the next deployment is green.

The result of the drill is recorded in the [CHANGELOG](../CHANGELOG.md). The `Roll back` step ends with `exit 1` on purpose: a rolled-back deployment is still a failed deployment.

## Troubleshooting

| Symptom | Likely cause |
|---|---|
| `ssh: Permission denied` in `Deploy` | wrong `DEPLOY_SSH_KEY`, or the public key was not substituted in cloud-init |
| `Host key verification failed` | `DEPLOY_KNOWN_HOSTS` empty or from a different VM (rebuilt instance: rescan) |
| `pull access denied` / `unauthorized` | the GHCR packages are still private (step 8.3) |
| Browser or curl times out on 80/443 | missing ingress rule (step 3.5) or iptables not saved (`sudo iptables -S INPUT` should list ports 80 and 443) |
| Certificate error | DNS name does not yet point at the VM IP, or Let's Encrypt rate limit (`docker compose logs web` on the VM) |
| Smoke test fails right after deploy | certificate issuance takes up to a minute; the test retries for 60 s, re-run the job |
| `/opt/app/deploy.sh` missing | private repository (raw download returns 404): install it with `scp` as in step 5 |
| Updating `deploy.sh` | the VM keeps its copy (public repository only; otherwise `scp` it again as in step 5): `sudo -u deploy curl -fsSL https://raw.githubusercontent.com/al-exd/montreal-contraventions/master/infra/deploy.sh -o /opt/app/deploy.sh` |
