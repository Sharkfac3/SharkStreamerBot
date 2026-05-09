---
id: phase-3-chunk-7-cross-reference-updates
type: refactor-prompt
phase: 3
chunk: 7
status: ready
---

# Phase 3 / Chunk 7 — Update Cross-References to SHARED-CONSTANTS.md

## Your Role

You are a scaffolding builder working on the SharkStreamerBot repository. You update agent scaffolding markdown files to fix cross-references after a file reorganization. You do not touch `.cs` runtime scripts.

## Repository Context

After Phase 3 Chunks 1–6, `Actions/SHARED-CONSTANTS.md` is now an index file. Domain constants live in `Actions/constants/<domain>.md`. Any AGENTS.md file that previously told agents to "see SHARED-CONSTANTS.md" for specific constants is now pointing to the wrong level — it should point directly to the relevant constants file.

This chunk updates those cross-references so agents load only the constants they need.

## Prerequisite

All Phase 3 Chunks 1–6 must show `done` in `progress.md`. Chunk 7 must show `pending`.

## What This Chunk Does

Audit all AGENTS.md files under `Actions/` for references to `SHARED-CONSTANTS.md` that can be made more specific. Update each reference to point to the relevant constants file.

**Do not change references that should remain general** (i.e., "for all constants see SHARED-CONSTANTS.md" is still valid as an index pointer — but "for OBS scene names see SHARED-CONSTANTS.md" should become "for OBS scene names see constants/stream-core.md").

## What This Chunk Does NOT Do

- Remove `SHARED-CONSTANTS.md` from any "Required Reading" chain — it is still valid as an index
- Touch any `.cs` files
- Touch contracts.md files
- Change anything outside `Actions/`

## Files to Read Before Starting

1. `Actions/SHARED-CONSTANTS.md` — the new index (post chunk 6)
2. `Actions/constants/stream-core.md` — to understand what it covers
3. `Actions/constants/mini-games.md`
4. `Actions/constants/commanders.md`
5. `Actions/constants/overlay-broker.md`
6. `Actions/constants/lotat.md`
7. `Actions/constants/effects.md`
8. `Projects/actions-scaffolding-refactor/progress.md`

Then audit these AGENTS.md files for specific-constant references:
- `Actions/Squad/AGENTS.md`
- `Actions/LotAT/AGENTS.md`
- `Actions/Commanders/AGENTS.md`
- `Actions/Overlay/AGENTS.md`
- `Actions/Rest Focus Loop/AGENTS.md`
- `Actions/XJ Drivethrough/AGENTS.md`
- `Actions/Destroyer/AGENTS.md`
- `Actions/Twitch Core Integrations/AGENTS.md`
- `Actions/AGENTS.md` (master)

## Step-by-Step Instructions

1. Read the 7 constants files to understand which domain each covers.

2. For each AGENTS.md listed above, read it and look for lines that:
   - Reference `SHARED-CONSTANTS.md` in the context of a specific domain (e.g., "see SHARED-CONSTANTS.md for mini-game lock state")
   - Reference specific global names, OBS sources, or timer names from SHARED-CONSTANTS.md inline

3. For each specific reference found, update it to point to the appropriate constants file. Examples:

   **Before:** `See [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for mini-game lock globals.`
   **After:** `See [Actions/constants/mini-games.md](../constants/mini-games.md) for mini-game lock globals.`

   **Before:** `See SHARED-CONSTANTS.md for OBS scene names.`
   **After:** `See [constants/stream-core.md](../constants/stream-core.md) for OBS scene names.`

4. Do NOT change generic "Required Reading" entries that point to `SHARED-CONSTANTS.md` as the overall constants reference — those are fine pointing to the index. Only update references that are domain-specific.

5. If an AGENTS.md has no specific constants references (only generic ones), leave it unchanged. Note it in the progress.md Notes section.

6. After all updates, read each modified AGENTS.md to confirm the references resolve correctly relative to that file's location.

7. Update `progress.md`: Phase 3 chunk 7 → `done`, update "Last updated". Add note: "Phase 3 complete — SHARED-CONSTANTS.md is a clean index, cross-references updated. Phase 4 may begin."

## Output Requirements

- Every domain-specific SHARED-CONSTANTS.md reference in audited AGENTS.md files points to the correct `constants/<domain>.md` file
- Generic index references to `SHARED-CONSTANTS.md` are preserved
- No broken relative paths (verify path depth — files in subfolders need `../constants/` not `constants/`)
- `progress.md` Phase 3 chunk 7 shows `done`, Phase 3 completion note added

## Validation Checklist

- [ ] All 9 AGENTS.md files audited
- [ ] Domain-specific references updated to point to constants files
- [ ] Relative paths are correct for each file's location
- [ ] Generic "see SHARED-CONSTANTS.md" references preserved where appropriate
- [ ] `progress.md` updated with Phase 3 completion note

## Handoff to Phase 4

Phase 4 adds role-scoped entry points. It does not modify any constants files or AGENTS.md files — only adds new scope files to role folders.
