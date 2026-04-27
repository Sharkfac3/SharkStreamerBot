# Prompt 102b — Actions Helper Snippets Split (Execute)

## Agent

Pi (manual copy/paste by operator).

## Purpose

Execute the split designed in prompt 102. Create `Actions/Helpers/` cluster, rewrite `Actions/HELPER-SNIPPETS.md` as thin compatibility index, update targeted cross-references, register new files in manifest.

Operator-ratified decisions from `Projects/agent-reflow/findings/102-helper-snippets-split-design.md`:

1. **Index fate**: Option A — keep `Actions/HELPER-SNIPPETS.md` as thin compatibility index pointing at new helper files.
2. **Mix It Up boundary**: keep `Actions/Helpers/mixitup-command-api.md` for Streamer.bot C# helper; cross-link to `Tools/MixItUp/AGENTS.md` for payload/tooling authority.
3. **Mini-game contract location**: dedicated `Actions/Helpers/mini-game-contract.md`, not folded into `Actions/Squad/AGENTS.md`.
4. **Manifest granularity**: simple `co_locations` entries only. No skill/domain promotion.
5. **Cross-reference aggressiveness**: targeted updates only (Layer 2 from design). Broad refs stay pointed at thin `HELPER-SNIPPETS.md`.
6. **Comment-only `.cs` updates**: allowed, scoped to comments only. One known site: `Actions/Overlay/broker-publish.cs`.

## Preconditions

- Prompt 102 complete; design doc at `Projects/agent-reflow/findings/102-helper-snippets-split-design.md` exists
- Operator has ratified the 6 decisions listed in Purpose
- Validator green: `python3 Tools/AgentTree/validate.py` returns 0 failures
- Read the design doc, prompts 100/101 handoffs, and audit finding 1.1 first

## Scope

Creates:
- `Actions/Helpers/README.md` — folder index
- `Actions/Helpers/mini-game-lock.md`
- `Actions/Helpers/mini-game-contract.md`
- `Actions/Helpers/mixitup-command-api.md`
- `Actions/Helpers/chat-input.md`
- `Actions/Helpers/obs-scenes.md`
- `Actions/Helpers/cph-api-signatures.md`
- `Actions/Helpers/timers.md`
- `Actions/Helpers/json-no-external-libraries.md`

Rewrites:
- `Actions/HELPER-SNIPPETS.md` — body replaced with thin compatibility index pointing at new helper files

Edits (manifest):
- `.agents/manifest.json` — add 9 `co_locations` entries (one per new helper doc)

Edits (Layer 2 targeted cross-references — see Steps for full list):
- `Actions/Overlay/README.md`
- `Actions/Overlay/AGENTS.md`
- `Actions/Rest Focus Loop/AGENTS.md`
- `Actions/Temporary/AGENTS.md`
- `Actions/Voice Commands/AGENTS.md`
- `Actions/Twitch Channel Points/AGENTS.md`
- `Actions/Squad/README.md`

Comment-only edit (one file):
- `Actions/Overlay/broker-publish.cs` — comment update only, no runtime logic change

## Out-of-scope

- No runtime `.cs` logic changes. Only the comment in `Actions/Overlay/broker-publish.cs` touches a `.cs` file
- No broad `HELPER-SNIPPETS.md` reference rewrites in `.agents/_shared/project.md`, `.agents/_shared/conventions.md`, `Docs/ONBOARDING.md`, `Docs/Architecture/repo-structure.md`, `Tools/StreamerBot/AGENTS.md`, `.agents/workflows/sync.md`, `.agents/workflows/validation.md`, `.agents/roles/streamerbot-dev/role.md`, `Docs/AGENT-WORKFLOW.md` — those keep pointing at thin index
- No new manifest skill or domain entries; only `co_locations`
- No edits to `Actions/SHARED-CONSTANTS.md` body content (back-links optional and skipped this pass)
- No git operations

## Steps

### Layer 1 — Create helper files

1. Create `Actions/Helpers/` directory.

2. Create `Actions/Helpers/README.md` with frontmatter from design § 1 and body containing:
   - Purpose (folder index for reusable Streamer.bot C# helpers)
   - Table of helper files with one-line description per file
   - Note: `Actions/HELPER-SNIPPETS.md` retained as compatibility index for legacy references

3. For each of the 8 concept files, copy the corresponding H2 section verbatim from `Actions/HELPER-SNIPPETS.md`. Add frontmatter per design. Files + source sections:

| New file | Absorbs from `Actions/HELPER-SNIPPETS.md` |
|---|---|
| `Actions/Helpers/mini-game-lock.md` | `## 1) Mini-game Lock Helper (Global, cross-feature)` (lines ~12–82) |
| `Actions/Helpers/mini-game-contract.md` | `## Required mini-game contract checklist` (lines ~1037–1043) |
| `Actions/Helpers/mixitup-command-api.md` | `## 2) Mix It Up Command API Helper` (lines ~83–220) |
| `Actions/Helpers/chat-input.md` | `## 3) Chat Message Input Helper (message/rawInput)` (lines ~221–273) |
| `Actions/Helpers/obs-scenes.md` | `## 4) OBS Scene Switching` (lines ~274–313) |
| `Actions/Helpers/cph-api-signatures.md` | `## 5) Verified CPH API Method Signatures` (lines ~314–380) |
| `Actions/Helpers/timers.md` | `## 6) Timer Management` (lines ~381–465) |
| `Actions/Helpers/json-no-external-libraries.md` | `## 7) JSON Parse / Serialize Helper (No External Libraries)` (lines ~466–1036) |

4. Within each new helper file, add cross-link section pointing at related helpers per design's "Cross-references out/dependencies" column. Specifically:
   - `mini-game-lock.md` → `mini-game-contract.md`, `Actions/SHARED-CONSTANTS.md` Mini-game Lock section, `Actions/Twitch Core Integrations/stream-start.cs`
   - `mini-game-contract.md` → `mini-game-lock.md`, `Actions/SHARED-CONSTANTS.md`
   - `mixitup-command-api.md` → `Tools/MixItUp/AGENTS.md` (payload/tooling authority), `Actions/SHARED-CONSTANTS.md` Bits / Mix It Up Unlock Pacing
   - `chat-input.md` → `cph-api-signatures.md`
   - `obs-scenes.md` → `cph-api-signatures.md`, `Actions/SHARED-CONSTANTS.md` OBS / Stream Mode
   - `cph-api-signatures.md` → `timers.md`, `obs-scenes.md`
   - `timers.md` → `cph-api-signatures.md`, `Actions/SHARED-CONSTANTS.md` timer names
   - `json-no-external-libraries.md` → `mixitup-command-api.md` (note: Mix It Up helper still uses `System.Text.Json`), `Actions/Overlay/AGENTS.md`

### Layer 2 — Rewrite `Actions/HELPER-SNIPPETS.md` as thin index

5. Replace `Actions/HELPER-SNIPPETS.md` body with:
   - Frontmatter (preserve existing or align with `Actions/Helpers/` files; mark `status: active`, type appropriate)
   - Short compatibility note ("snippets reorganized by concept under `Actions/Helpers/`; this file retained as compatibility index")
   - Mapping table — old H2 section title → new file path
   - Link to `Actions/Helpers/README.md` as authoritative index

6. Delete the 8 H2 section bodies + the contract checklist body. Final `Actions/HELPER-SNIPPETS.md` should be ≤80 lines.

### Layer 3 — Targeted cross-reference updates (Layer 2 from design)

7. Update each file below per design's Cross-Reference Update Plan table:

| File | Change |
|---|---|
| `Actions/Overlay/README.md` | Replace `Actions/HELPER-SNIPPETS.md § 7` references with `Actions/Helpers/json-no-external-libraries.md` |
| `Actions/Overlay/AGENTS.md` | Replace generic helper-snippets pointer (where it discusses complex JSON serialization) with `Actions/Helpers/json-no-external-libraries.md` |
| `Actions/Rest Focus Loop/AGENTS.md` | Replace verified-timer-methods/helper-patterns refs with `Actions/Helpers/timers.md` and `Actions/Helpers/cph-api-signatures.md` |
| `Actions/Temporary/AGENTS.md` | Replace Mix It Up API helper-pattern ref with `Actions/Helpers/mixitup-command-api.md` |
| `Actions/Voice Commands/AGENTS.md` | Replace direct-OBS-scene-setter discussion ref with `Actions/Helpers/obs-scenes.md` |
| `Actions/Twitch Channel Points/AGENTS.md` | Replace direct-OBS-methods / no-reflection ref with `Actions/Helpers/obs-scenes.md` |
| `Actions/Squad/README.md` | Fix stale "mini-game contract is in root README" statement; point at `Actions/Helpers/mini-game-contract.md` |
| `Actions/Squad/AGENTS.md` | Add link to `Actions/Helpers/mini-game-contract.md` and `Actions/Helpers/mini-game-lock.md` where local mini-game guidance currently lives |

### Layer 4 — Comment-only `.cs` update

8. Edit `Actions/Overlay/broker-publish.cs` comments only:
   - Find any comment referencing `Actions/HELPER-SNIPPETS.md § 7` or generic helper-snippets path
   - Replace with comment pointing at `Actions/Helpers/json-no-external-libraries.md`
   - **Do not change any executable code, no method bodies, no using directives, no signatures**
   - Verify with diff: only comment lines changed

### Layer 5 — Manifest update

9. Add 9 `co_locations` entries to `.agents/manifest.json` (one per new helper file). Each entry:
   ```json
   {
     "path": "Actions/Helpers/<file>.md",
     "owner": "streamerbot-dev",
     "status": "active"
   }
   ```
   No new entries in `skills` or `domains`. Order entries alphabetically by path or per existing manifest convention.

### Layer 6 — Validate

10. Run `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/102b-validator.failures.txt`. Expect 0 failures across all checks.

11. If failures appear, iterate within prompt scope. Common expected fix areas: frontmatter mismatches in new files, manifest entry typos, broken cross-links inside new helpers.

### Layer 7 — Final checks

12. Confirm every H2 section from old `Actions/HELPER-SNIPPETS.md` has a destination in the new files (no orphan content).

13. Confirm `Actions/HELPER-SNIPPETS.md` line count ≤80.

14. Confirm `Actions/Overlay/broker-publish.cs` diff is comment-only (run `git diff Actions/Overlay/broker-publish.cs` and inspect — operator commits, agent does not).

15. Write handoff.

## Validator / Acceptance

- `python3 Tools/AgentTree/validate.py` exits 0
- 9 new files exist under `Actions/Helpers/` with required frontmatter
- `Actions/HELPER-SNIPPETS.md` ≤80 lines, contains compatibility mapping table only
- Manifest has 9 new `co_locations` entries; no new `skills` or `domains` entries
- 8 targeted cross-references updated per Layer 3 table
- `Actions/Overlay/broker-publish.cs` diff is comment-only
- Broad refs (in `.agents/_shared/`, `Docs/`, workflows, role docs) **not** modified — confirm by git diff scope

## Handoff

Write `Projects/agent-reflow/handoffs/102b-actions-helper-snippets-split-execute.handoff.md` per template. Include:

- File creation summary (9 new files + line counts)
- `HELPER-SNIPPETS.md` before/after line count
- Cross-reference updates applied (per Layer 3 table)
- Comment-only `.cs` edit summary (file + line numbers + before/after snippet)
- Manifest co_locations added (list of 9 entries)
- Final validator output
- Any orphan content discovered during step 12 (if any H2 section content didn't fit cleanly into target file)
- Streamer.bot paste targets: **N/A** — comment-only `.cs` change does not require Streamer.bot resync; helper docs are agent-tree only
- Open notes for operator: any references found during execution that weren't in design's Layer 3 list and should be flagged for follow-up
