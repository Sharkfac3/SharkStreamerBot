#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
OVERLAY_DIR="$SCRIPT_DIR/Apps/stream-overlay"

if [[ ! -f "$OVERLAY_DIR/package.json" ]]; then
  echo "Could not find Apps/stream-overlay/package.json. Run from the repo root." >&2
  exit 1
fi

if ! command -v pnpm >/dev/null 2>&1; then
  echo "pnpm not found. Install it: npm install -g pnpm" >&2
  exit 1
fi

cleanup() {
  local exit_code=$?

  if [[ -n "${BROKER_PID:-}" ]] && kill -0 "$BROKER_PID" >/dev/null 2>&1; then
    kill "$BROKER_PID" >/dev/null 2>&1 || true
  fi

  if [[ -n "${OVERLAY_PID:-}" ]] && kill -0 "$OVERLAY_PID" >/dev/null 2>&1; then
    kill "$OVERLAY_PID" >/dev/null 2>&1 || true
  fi

  exit "$exit_code"
}
trap cleanup EXIT INT TERM

echo "Starting broker on port 8765..."
(
  cd "$OVERLAY_DIR"
  pnpm dev:broker
) &
BROKER_PID=$!

echo "Starting overlay on http://localhost:5173 ..."
(
  cd "$OVERLAY_DIR"
  pnpm dev:overlay
) &
OVERLAY_PID=$!

echo "Both processes started in the background."
echo "Point OBS browser source to http://localhost:5173"
echo "Press Ctrl+C to stop both processes."

wait "$BROKER_PID" "$OVERLAY_PID"
