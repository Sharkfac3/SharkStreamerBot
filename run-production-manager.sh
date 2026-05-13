#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
APP_DIR="$SCRIPT_DIR/Apps/production-manager"

if [[ ! -f "$APP_DIR/package.json" ]]; then
  echo "Could not find Apps/production-manager/package.json. Run from the repo root." >&2
  exit 1
fi

if ! command -v npm >/dev/null 2>&1; then
  echo "npm not found. Install Node.js/npm first." >&2
  exit 1
fi

echo "Starting production-manager on http://127.0.0.1:5174 ..."
cd "$APP_DIR"
npm run dev
