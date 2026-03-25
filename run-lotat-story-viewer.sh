#!/usr/bin/env bash
set -euo pipefail

# Always resolve paths relative to this script so the viewer can be launched
# from anywhere without import-path issues.
SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

cd "$SCRIPT_DIR"
exec python3 -m uvicorn --app-dir "$SCRIPT_DIR" Tools.LotAT.story_viewer:app --reload
