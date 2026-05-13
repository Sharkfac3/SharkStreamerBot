#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
APPS_DIR="$SCRIPT_DIR/Apps"
STREAM_OVERLAY_DIR="$APPS_DIR/stream-overlay"
INFO_SERVICE_DIR="$APPS_DIR/info-service"
PRODUCTION_MANAGER_DIR="$APPS_DIR/production-manager"

for app_dir in "$STREAM_OVERLAY_DIR" "$INFO_SERVICE_DIR" "$PRODUCTION_MANAGER_DIR"; do
  if [[ ! -f "$app_dir/package.json" ]]; then
    echo "Could not find $app_dir/package.json. Run this script from the repo root." >&2
    exit 1
  fi
done

if ! command -v pnpm >/dev/null 2>&1; then
  echo "pnpm not found. Install it: npm install -g pnpm" >&2
  exit 1
fi

if ! command -v npm >/dev/null 2>&1; then
  echo "npm not found. Install Node.js/npm first." >&2
  exit 1
fi

cleanup() {
  local exit_code=$?

  for pid_var in BROKER_PID OVERLAY_PID INFO_SERVICE_PID PRODUCTION_MANAGER_PID; do
    local pid="${!pid_var:-}"
    if [[ -n "$pid" ]] && kill -0 "$pid" >/dev/null 2>&1; then
      kill "$pid" >/dev/null 2>&1 || true
    fi
  done

  exit "$exit_code"
}
trap cleanup EXIT INT TERM

echo "Starting stream-overlay broker on http://127.0.0.1:8765 ..."
(
  cd "$STREAM_OVERLAY_DIR"
  pnpm dev:broker
) &
BROKER_PID=$!

echo "Starting stream-overlay overlay on http://127.0.0.1:5173 ..."
(
  cd "$STREAM_OVERLAY_DIR"
  pnpm dev:overlay
) &
OVERLAY_PID=$!

echo "Starting info-service on http://127.0.0.1:8766 ..."
(
  cd "$INFO_SERVICE_DIR"
  npm run dev
) &
INFO_SERVICE_PID=$!

echo "Starting production-manager on http://127.0.0.1:5174 ..."
(
  cd "$PRODUCTION_MANAGER_DIR"
  npm run dev
) &
PRODUCTION_MANAGER_PID=$!

echo
echo "All Apps processes started in the background:"
echo "- stream-overlay broker: http://127.0.0.1:8765"
echo "- stream-overlay overlay: http://127.0.0.1:5173"
echo "- info-service: http://127.0.0.1:8766"
echo "- production-manager: http://127.0.0.1:5174"
echo "Press Ctrl+C to stop them all."

wait "$BROKER_PID" "$OVERLAY_PID" "$INFO_SERVICE_PID" "$PRODUCTION_MANAGER_PID"
