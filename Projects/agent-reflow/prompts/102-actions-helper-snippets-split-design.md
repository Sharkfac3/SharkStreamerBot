# Prompt 102 — Actions Helper Snippets Split (Design Only)

## Agent

Pi (manual copy/paste by operator).

## Purpose

Design conceptual split for `Actions/HELPER-SNIPPETS.md` (1,044 lines, 8 unrelated helper domains). Audit finding 1.1: junior reader cannot find right helper without reading whole file.

**Design only — no edits to `Actions/HELPER-SNIPPETS.md` or any helper-related code.** Operator approves design, then a later prompt (`102b-actions-helper-snippets-split-execute`) performs the split if approved.

Reason for design-then-execute split: HELPER-SNIPPETS is the highest-traffic reference doc for `Actions/*.cs` work. Splitting it touches every script's referenced helper. Operator should review boundary decisions before execution.

Bundles audit findings: 1.1.

## Preconditions

- Prompt 101 complete (stale cleanup landed)
- Validator green
- Read `findings/99-audit.md` finding 1.1 first

## Scope

Reads:
- `Actions/HELPER-SNIPPETS.md` (full)
- `Actions/SHARED-CONSTANTS.md` (for cross-reference)
- All `Actions/*/AGENTS.md` (to identify which helpers are referenced where)
- `.agents/_shared/conventions.md`

Writes:
- `Projects/agent-reflow/findings/102-helper-snippets-split-design.md` (design proposal)

## Out-of-scope

- No edits to `Actions/HELPER-SNIPPETS.md`
- No new helper files yet
- No edits to `Actions/*.cs` runtime code
- No edits to `Actions/*/AGENTS.md` cross-references
- No git operations

## Steps

1. Read `Actions/HELPER-SNIPPETS.md` end-to-end. Map every H2 section to its concept domain.

2. For each H2 section, capture:
   - Section title
   - Approximate line range
   - Concept domain (mini-game lock, Mix It Up API, OBS scenes, CPH signatures, timers, JSON helpers, chat input, mini-game contract, etc.)
   - Cross-references **into** the section from other docs (which `Actions/*/AGENTS.md`, `*.cs`, etc. mention it)
   - Cross-references **out of** the section (which other helpers it depends on)

3. Propose split target files (per audit suggestion, refine if better boundaries surface):
   - `Actions/Helpers/mini-game-lock.md`
   - `Actions/Helpers/mixitup-command-api.md` (or cross-link to `Tools/MixItUp/AGENTS.md` — decide)
   - `Actions/Helpers/chat-input.md`
   - `Actions/Helpers/obs-scenes.md`
   - `Actions/Helpers/cph-api-signatures.md`
   - `Actions/Helpers/timers.md`
   - `Actions/Helpers/json-no-external-libraries.md`
   - Mini-game contract checklist → relocation target (`Actions/Squad/AGENTS.md` or new `Actions/Helpers/mini-game-contract.md`)

4. For each proposed file, document:
   - Source H2 sections it absorbs
   - New file's required frontmatter (per `_shared/conventions.md`)
   - New file's manifest co-location entry (id, type, status)
   - Cross-references that need updating in **other** docs to point at new location

5. Document `Actions/HELPER-SNIPPETS.md` post-split fate:
   - **Option A**: keep as thin index pointing at all `Actions/Helpers/*.md` files
   - **Option B**: delete entirely; `Actions/Helpers/README.md` becomes the index
   - Recommend one, justify

6. Document open decisions for operator:
   - Mix It Up section: keep in `Actions/Helpers/` or cross-link to `Tools/MixItUp/AGENTS.md`?
   - Mini-game contract: live in `Actions/Squad/AGENTS.md` or new dedicated file?
   - Index file fate (Option A vs B)

7. Write design doc with sections: Overview, Section Inventory, Proposed Splits, Cross-Reference Update Plan, Open Decisions.

8. Write handoff. Note: execution prompt (`102b`) is a separate prompt operator must spawn after ratifying design.

## Validator / Acceptance

- `Projects/agent-reflow/findings/102-helper-snippets-split-design.md` exists
- Every H2 section in `Actions/HELPER-SNIPPETS.md` has a destination in proposed splits (no orphan content)
- Open decisions section non-empty
- No edits to `Actions/HELPER-SNIPPETS.md` (verify with file mtime / line count unchanged)

## Handoff

Per template. Include:
- Design doc location
- Open decisions list (operator must resolve before `102b` runs)
- Estimated execution scope (file count, doc-update count)
- Note: execution does not happen in this prompt
