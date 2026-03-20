# Validation

Pre-commit validation for SharkStreamerBot. Catches constant drift, story JSON schema errors, and Pi skill-wrapper regressions before they reach `main`.

## Setup (run once)

```bash
python3 Tools/StreamerBot/Validation/install-hooks.py
```

This installs a `.git/hooks/pre-commit` that runs `validate.py` automatically before every commit using the same Python interpreter that installed the hook.

## Manual run

```bash
python3 Tools/StreamerBot/Validation/validate.py
```

Run from anywhere inside the repo. It finds the root automatically.

## Sync routing docs from the manifest

```bash
python3 Tools/StreamerBot/Validation/sync-routing-docs.py
```

This rewrites the generated routing tables in:
- `AGENTS.md`
- `.agents/ENTRY.md`
- `.pi/skills/README.md`

## What it checks

### Check 1 — File existence
All files listed in `Actions/SHARED-CONSTANTS.md` "Used in:" blocks must exist.
Catches file renames or deletions that weren't reflected in the constants doc.

### Check 2 — Value drift
For each constant defined in `Actions/SHARED-CONSTANTS.md`, if the listed `.cs`
files also declare that constant by name, their string values must match.
Catches silent renames where the code changed but the doc didn't (or vice versa).

### Check 3 — Story JSON schema
All `.json` files in `Creative/WorldBuilding/Storylines/` must conform to the
shared story schema defined in `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`.
Catches malformed story files before they reach the technical agent or the C# engine.

### Check 4 — Pi skill wrapper integrity
Validates the agent-facing Pi skill layer by checking:
- every Pi skill file has valid frontmatter
- every Pi skill `name` matches its parent directory and stays Pi-safe
- every `.agents/roles/<role>/role.md` has a matching `.pi/skills/<role>/SKILL.md`
- `.agents/routing-manifest.json` is a valid primary routing contract
- `AGENTS.md` quick routing block matches the routing manifest
- `.agents/ENTRY.md` and `.pi/skills/README.md` match the routing manifest's generated Roles table
- `.pi/skills/README.md` matches the routing manifest's generated Meta Wrappers table
- `.pi/skills/README.md` alias inventory matches the routing manifest's generated compatibility alias table
- manifest collisions and duplicate routing entries are rejected
- no orphan Pi wrapper directories exist outside the routing manifest
- every manifest-declared canonical wrapper is surfaced from its required route files
- alias wrappers use the expected migrated-alias template and only point to canonical wrappers
- repo markdown does not contain stale or nested Pi wrapper references
- `.pi/skills/` references only point to existing flat Pi wrappers

This catches agent-routing regressions before they land in the repo.

## Exit codes

| Code | Meaning |
|------|---------|
| `0`  | All checks passed — commit proceeds |
| `1`  | One or more checks failed — commit blocked |

## Fixing failures

**File not found:** Update the "Used in:" list in `Actions/SHARED-CONSTANTS.md`
to reflect the current file path, or restore the file.

**Value drift:** Either update `Actions/SHARED-CONSTANTS.md` to match what's
in the code, or update the code to match the doc. Both directions are valid —
the key is that they agree.

**Story schema error:** Fix the field or structure in the story JSON file.
See `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
for the full schema reference.

**Pi skill wrapper failure:** Fix the wrapper name, routing manifest, role parity, or wrapper reference so it follows the flat Pi wrapper contract documented in `.pi/skills/README.md`.
