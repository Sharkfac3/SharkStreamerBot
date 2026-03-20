# Validation

## Pre-Commit Hooks

Install with: `python3 Tools/StreamerBot/Validation/install-hooks.py`

The pre-commit hook runs `validate.py` which checks for:
- SHARED-CONSTANTS drift (global variable names used in scripts that are not in SHARED-CONSTANTS.md)
- Story JSON schema compliance
- Pi skill wrapper integrity and `.agents/` ↔ `.pi/skills/` role parity
- `.agents/routing-manifest.json` as the primary routing contract
- `.agents/ENTRY.md` ↔ `.pi/skills/README.md` Roles table parity
- routing-manifest-backed alias inventory and canonical wrapper route-surface coverage
- manifest collisions, duplicate routing entries, and stale Pi wrapper references across repo markdown

## Manual Validation Script

`Tools/StreamerBot/Validation/validate.py`

Run directly for on-demand validation passes. See `Tools/StreamerBot/Validation/README.md` for options.

## Routing Doc Sync Script

`Tools/StreamerBot/Validation/sync-routing-docs.py`

Run this after editing `.agents/routing-manifest.json` to rewrite generated routing tables before validating.

## What to Validate Before Committing

- [ ] Any new global variable is in `Actions/SHARED-CONSTANTS.md` and reset in `stream-start.cs`
- [ ] No hardcoded string literals for OBS scene names, timer names, or variable names (use SHARED-CONSTANTS)
- [ ] Script syntax is clean (no C# compile errors)
- [ ] Feature README is updated if behavior changed
- [ ] Any new `.agents/` role has a matching `.pi/skills/<role>/SKILL.md`
- [ ] `.agents/routing-manifest.json` is updated for any routing change
- [ ] `python3 Tools/StreamerBot/Validation/sync-routing-docs.py` was run after routing-contract edits
- [ ] `.pi/skills/README.md` Roles table still matches `.agents/ENTRY.md`
- [ ] Any new Pi-exposed sub-skill wrapper uses the flat `<role>-<subskill>` naming pattern
- [ ] `.pi/skills/README.md` generated tables and route surfacing still align with `.agents/routing-manifest.json`
- [ ] Pi wrapper references point only to existing flat wrappers

## SHARED-CONSTANTS Drift

If validation flags a SHARED-CONSTANTS drift:
1. Stop — do not commit
2. Add the new constant to `Actions/SHARED-CONSTANTS.md`
3. Add the reset to `Actions/Twitch Core Integrations/stream-start.cs`
4. Re-run validation
5. Then commit

## Pi Wrapper Drift

If validation flags Pi wrapper drift:
1. Stop — do not commit
2. Read `.pi/skills/README.md` for the canonical wrapper contract
3. Fix the wrapper name, target, or missing role wrapper
4. Re-run validation
5. Then commit
