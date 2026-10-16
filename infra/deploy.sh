#!/usr/bin/env bash
# Forced command of the CD deploy key (see infra/cloud-init.yml).
# The client chooses the action via the SSH command: "deploy" (tar with compose.yml and env on stdin) or "rollback".
set -euo pipefail

app_dir="${APP_DIR:-/opt/app}"
action="${SSH_ORIGINAL_COMMAND:-}"

cd "$app_dir"

start_stack() {
  docker compose pull
  docker compose up -d --remove-orphans
}

case "$action" in
  deploy)
    stage="$(mktemp -d)"
    trap 'rm -rf "$stage"' EXIT
    tar -x -C "$stage" -f - compose.yml env
    if [ -f .env ]; then
      cp -p .env .env.previous
    fi
    install -m 600 "$stage/env" .env
    install -m 644 "$stage/compose.yml" compose.yml
    start_stack
    ;;
  rollback)
    if [ ! -f .env.previous ]; then
      echo "no .env.previous: nothing to roll back to" >&2
      exit 1
    fi
    install -m 600 .env.previous .env
    start_stack
    ;;
  *)
    echo "unsupported action: ${action:-<none>}" >&2
    exit 2
    ;;
esac
