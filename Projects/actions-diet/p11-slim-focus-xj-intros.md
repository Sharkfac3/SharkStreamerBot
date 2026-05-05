Edit these 3 files.

FILES:
- `Actions/Rest Focus Loop/AGENTS.md` (~354 lines, target ~60 lines) — no ACTION-CONTRACTS block
- `Actions/XJ Drivethrough/AGENTS.md` (~242 lines, target ~80 lines) — HAS ACTION-CONTRACTS block (already compressed in p03)
- `Actions/Intros/AGENTS.md` (~209 lines, target ~70 lines) — HAS ACTION-CONTRACTS block (already compressed in p03)

For XJ Drivethrough and Intros: DO NOT touch the `<!-- ACTION-CONTRACTS:START -->` / `<!-- ACTION-CONTRACTS:END -->` block. Only edit prose sections outside those markers.

For all three files, apply the same pattern to prose sections:

REMOVE or REPLACE:
1. Ownership section → 1–2 lines + pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. Keep folder-specific refs alongside it.
3. Local Workflow: remove universal rules. Keep only non-obvious folder-specific behavior.
4. Script Reference tables → replace with compact one-line-per-script summary table.
5. Validation / Boundaries / Handoff → pointer to `Actions/AGENTS.md`.

FOLDER-SPECIFIC CONTENT TO KEEP:

**Rest Focus Loop:**
- Keep the phase sequence: `rest-focus-loop-start.cs` starts the loop → timer fires `rest-focus-pre-focus-end.cs` → `rest-focus-focus-end.cs` → timer fires `rest-focus-pre-rest-end.cs` → `rest-focus-rest-end.cs` → loops back. This phase order is the key non-obvious fact.
- Compact summary: 5-row table, one per script, with the phase it handles.

**XJ Drivethrough:**
- Keep any XJ-specific routing or product/content note that's in the prose (not in the contract block).
- Compact summary table for scripts not covered by the contract block (if any exist outside it).

**Intros:**
- Keep any intro-specific trigger routing (first-chat-intro vs redeem-capture distinction).
- Compact summary table for scripts not covered by the contract block.

After editing all three, run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Must pass with 0 errors. (XJ Drivethrough and Intros contracts were not touched, so this is a sanity check.)
