Edit these 3 files. None have ACTION-CONTRACTS blocks — no validator constraint.

FILES:
- `Actions/Twitch Bits Integrations/AGENTS.md` (~491 lines, target ~80 lines)
- `Actions/Twitch Hype Train/AGENTS.md` (~406 lines, target ~60 lines)
- `Actions/Twitch Channel Points/AGENTS.md` (~263 lines, target ~50 lines)

For each file, apply the same pattern:

REMOVE or REPLACE:
1. Ownership section → replace with 1–2 lines: owner is `streamerbot-dev`, chain to `brand-steward` for public copy. Pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. Keep any folder-specific refs alongside it.
3. Local Workflow: remove universal script rules. Keep only non-obvious folder-specific behavior.
4. Large Script Reference tables → replace with compact one-line-per-script summary table.
5. Validation / Boundaries / Handoff → replace with pointer to `Actions/AGENTS.md`.

FOLDER-SPECIFIC CONTENT TO KEEP:

**Twitch Bits Integrations:**
- Keep the tier routing logic: which bit threshold maps to which script (tier 1/2/3/4 thresholds). These are non-obvious values.
- Compact summary table: script | bit tier/threshold | key effect (OBS source, overlay, etc.)

**Twitch Hype Train:**
- Keep the phase sequence note: start → level-up (repeatable) → end. Non-obvious that level-up fires multiple times.
- Compact summary table: `hype-train-start.cs`, `hype-train-level-up.cs`, `hype-train-end.cs` with one-line purpose each.

**Twitch Channel Points:**
- Only 2 scripts (disco-party, explain-current-task). No large table needed.
- Keep any channel-point reward ID or non-obvious routing note.
- Compact summary: 2-row table, script + purpose.

TARGET STRUCTURE for each file:
```
--- frontmatter ---
# [Name] — Agent Guide
## Purpose (2–3 lines)
## When to Activate (file list)
## Ownership (1–2 lines + pointer)
## Folder-Specific Rules (non-obvious only, ≤6 lines)
## Script Summary (compact table)
```
