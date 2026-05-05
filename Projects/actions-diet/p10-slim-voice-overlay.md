Edit these 2 files. Neither has an ACTION-CONTRACTS block — no validator constraint.

FILES:
- `Actions/Voice Commands/AGENTS.md` (~370 lines, target ~60 lines)
- `Actions/Overlay/AGENTS.md` (~396 lines, target ~60 lines)

For each file, apply the same pattern:

REMOVE or REPLACE:
1. Ownership section → 1–2 lines + pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. Keep folder-specific refs alongside it (for Overlay: keep `Actions/Helpers/overlay-broker.md` — it's directly relevant and specific).
3. Local Workflow: remove universal rules. Keep only non-obvious folder-specific behavior.
4. Large Script Reference tables → replace with compact one-line-per-script summary table.
5. Validation / Boundaries / Handoff → pointer to `Actions/AGENTS.md`.

FOLDER-SPECIFIC CONTENT TO KEEP:

**Voice Commands:**
- Keep the two-category distinction: `mode-*.cs` scripts change stream mode state; `scene-*.cs` scripts change OBS scenes. This is non-obvious and explains the naming pattern.
- Compact summary table: script | category (mode/scene) | what it changes.

**Overlay:**
- Keep the reference to `Actions/Helpers/overlay-broker.md` for the EnsureOverlayBrokerConnected pattern. The broker connection logic lives there; the Overlay folder uses it.
- Keep: `broker-connect.cs` and `broker-disconnect.cs` manage the WebSocket connection; `broker-publish.cs` sends messages; `test-overlay.cs` is a dev/debug tool.
- Compact summary table: 4 rows, one per script.

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
