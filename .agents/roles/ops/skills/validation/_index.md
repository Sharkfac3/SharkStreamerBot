# Validation

## Pre-Commit Hooks

Install with: `python Tools/StreamerBot/Validation/install-hooks.py`

The pre-commit hook runs `validate.py` which checks for:
- SHARED-CONSTANTS drift (global variable names used in scripts that are not in SHARED-CONSTANTS.md)
- Story JSON schema compliance

## Manual Validation Script

`Tools/StreamerBot/Validation/validate.py`

Run directly for on-demand validation passes. See `Tools/StreamerBot/Validation/README.md` for options.

## What to Validate Before Committing

- [ ] Any new global variable is in `Actions/SHARED-CONSTANTS.md` and reset in `stream-start.cs`
- [ ] No hardcoded string literals for OBS scene names, timer names, or variable names (use SHARED-CONSTANTS)
- [ ] Script syntax is clean (no C# compile errors)
- [ ] Feature README is updated if behavior changed

## SHARED-CONSTANTS Drift

If validation flags a SHARED-CONSTANTS drift:
1. Stop — do not commit
2. Add the new constant to `Actions/SHARED-CONSTANTS.md`
3. Add the reset to `Actions/Twitch Core Integrations/stream-start.cs`
4. Re-run validation
5. Then commit
