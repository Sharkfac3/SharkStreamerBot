# Project: Kill manifest.json

## Goal

Remove `.agents/manifest.json` and `.agents/manifest.schema.json` from the repo. They duplicate information already present in AGENTS.md frontmatter across the codebase. No agent reads the JSON at runtime — agents read AGENTS.md files directly.

## Execution Order

Run these prompts in order. Each is self-contained and can be sent to a separate agent session.

| Step | File | What it does |
|---|---|---|
| 1 | [01-update-docs.md](01-update-docs.md) | Remove manifest references from AGENTS.md, ENTRY.md, conventions.md, and AgentTree/AGENTS.md |
| 2 | [02-rewrite-validator.md](02-rewrite-validator.md) | Rewrite validate.py to scan AGENTS.md frontmatter instead of manifest.json |
| 3 | [03-delete-manifest.md](03-delete-manifest.md) | Delete manifest.json and manifest.schema.json |
| 4 | [04-verify.md](04-verify.md) | Run validator, check for broken references, confirm clean state |

## Why

- manifest.json is 1868 lines duplicating info in ~33 AGENTS.md files
- No agent reads it at runtime; only validate.py and generated doc blocks consume it
- Every new domain/role/workflow requires updating both AGENTS.md frontmatter AND manifest — drift-prone
- AGENTS.md frontmatter is already the real source of truth
