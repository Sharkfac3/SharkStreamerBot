# Validation

## Routing Doc Sync Script

`Tools/StreamerBot/Validation/sync-routing-docs.py`

Run this after editing `.agents/routing-manifest.json` to rewrite generated routing tables in `AGENTS.md`, `.agents/ENTRY.md`, and `.pi/skills/README.md`.

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
