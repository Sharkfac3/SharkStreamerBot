#!/usr/bin/env bash
set -euo pipefail

# Always resolve paths relative to this script so the viewer can be launched
# from anywhere without import-path issues.
SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

if [[ -x "$SCRIPT_DIR/.venv/bin/python3" ]]; then
  PYTHON="$SCRIPT_DIR/.venv/bin/python3"
elif [[ -x "$SCRIPT_DIR/.venv/bin/python" ]]; then
  PYTHON="$SCRIPT_DIR/.venv/bin/python"
elif command -v python3 >/dev/null 2>&1; then
  PYTHON="$(command -v python3)"
elif command -v python >/dev/null 2>&1; then
  PYTHON="$(command -v python)"
else
  echo "Could not find Python. Install Python in WSL or create .venv first." >&2
  exit 1
fi

cd "$SCRIPT_DIR"
exec "$PYTHON" -m uvicorn --app-dir "$SCRIPT_DIR" Tools.LotAT.story_viewer:app --reload
