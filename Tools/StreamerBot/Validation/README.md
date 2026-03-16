# Validation

Pre-commit validation for SharkStreamerBot. Catches constant drift and story JSON schema errors before they reach `main`.

## Setup (run once)

```
python Tools/StreamerBot/Validation/install-hooks.py
```

This installs a `.git/hooks/pre-commit` that runs `validate.py` automatically before every commit.

## Manual run

```
python Tools/StreamerBot/Validation/validate.py
```

Run from anywhere inside the repo. It finds the root automatically.

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
