Edit these 2 files. Neither has an ACTION-CONTRACTS block.

FILES:
- `Actions/Destroyer/AGENTS.md` (~110 lines, target ~40 lines)
- `Actions/Temporary/AGENTS.md` (~102 lines, target ~35 lines)

For each file, apply the same pattern:

REMOVE or REPLACE:
1. Ownership section → 1–2 lines + pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. If the folder has no other specific refs, this single entry is sufficient.
3. Local Workflow: remove all universal script rules. Keep only non-obvious folder-specific notes (if any).
4. Script Reference tables → replace with compact inline list or 2-row table.
5. Validation / Boundaries / Handoff → pointer to `Actions/AGENTS.md`.

FOLDER-SPECIFIC CONTENT TO KEEP:

**Destroyer:**
- Keep what `destroyer-spawn.cs` and `destroyer-move.cs` do — these are the two scripts, so the summary is small.
- If there's anything non-obvious about how Destroyer interacts with OBS, Squad state, or mini-game lock, keep it.

**Temporary:**
- This folder holds short-lived/experimental scripts. Keep a note that scripts here may be removed or promoted to a permanent folder — this is the key fact about the folder's nature.
- Currently holds `temp-focus-timer-end.cs` and `temp-focus-timer-start.cs`. Keep a 2-row script summary.
- If there's a note about why these are temporary (e.g., being evaluated before moving to Rest Focus Loop), keep it.

TARGET STRUCTURE for each file:
```
--- frontmatter ---
# [Name] — Agent Guide
## Purpose (2–3 lines)
## When to Activate (file list)
## Ownership (1 line + pointer)
## Script Summary (compact, ≤5 rows)
```

No Required Reading section needed if the only refs were the ones removed (SHARED-CONSTANTS and Helpers).
