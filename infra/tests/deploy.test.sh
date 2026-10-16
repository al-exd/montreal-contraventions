#!/usr/bin/env bash
# Tests for infra/deploy.sh using a fake docker and a temporary APP_DIR.
set -uo pipefail

here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
deploy="$here/../deploy.sh"
failures=0

setup() {
  work="$(mktemp -d)"
  export APP_DIR="$work/app" DOCKER_LOG="$work/docker.log"
  mkdir -p "$APP_DIR" "$work/bin"
  printf '#!/usr/bin/env bash\necho "$*" >> "$DOCKER_LOG"\n' > "$work/bin/docker"
  chmod +x "$work/bin/docker"
  export PATH="$work/bin:$PATH"
}

payload() { # payload <image-tag>: tar with compose.yml and env on stdout
  local d; d="$(mktemp -d)"
  echo "services: {}" > "$d/compose.yml"
  printf 'IMAGE_TAG=%s\nSITE_ADDRESS=example.test\n' "$1" > "$d/env"
  tar -C "$d" -cf - compose.yml env
}

run() { # run <action> [image-tag]
  if [ "$1" = deploy ]; then
    payload "$2" | SSH_ORIGINAL_COMMAND=deploy bash "$deploy" > /dev/null 2>&1
  else
    SSH_ORIGINAL_COMMAND="$1" bash "$deploy" < /dev/null > /dev/null 2>&1
  fi
}

check() { # check <description> <command...>
  local description="$1"; shift
  if "$@"; then echo "ok   $description"; else echo "FAIL $description"; failures=$((failures + 1)); fi
}

env_has() { grep -qx "$1" "$APP_DIR/.env"; }
mode_is() { [ "$(stat -c %a "$1")" = "$2" ]; }
docker_called() { grep -q "$1" "$DOCKER_LOG"; }
fails() { ! "$@"; }

setup
run deploy v1
check "deploy writes .env"                 env_has IMAGE_TAG=v1
check ".env is mode 600"                   mode_is "$APP_DIR/.env" 600
check "deploy installs compose.yml"        test -f "$APP_DIR/compose.yml"
check "deploy pulls images"                docker_called "compose pull"
check "deploy starts the stack"            docker_called "compose up -d"
check "first deploy has no .env.previous"  test ! -e "$APP_DIR/.env.previous"

run deploy v2
check "second deploy updates .env"         env_has IMAGE_TAG=v2
check "second deploy keeps previous .env"  grep -qx IMAGE_TAG=v1 "$APP_DIR/.env.previous"

run rollback
check "rollback restores previous .env"    env_has IMAGE_TAG=v1

setup
check "rollback without previous fails"    fails run rollback
check "unknown action fails"               fails run "rm -rf /"

[ "$failures" -eq 0 ] || { echo "$failures failed"; exit 1; }
echo "all passed"
