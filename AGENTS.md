# AGENTS.md

## 1) Project Context (Streamer.bot Execution Model)

This repository contains C# action scripts for **Streamer.bot**, plus local **BuildTools** utilities for external integrations (currently Mix It Up Developer API for overlay command discovery/export).

Important: these scripts are **not deployed automatically**. Each script is manually copied/pasted into Streamer.bot actions.

Because of this, all code changes must prioritize:
- copy/paste readiness,
- predictable runtime behavior in Streamer.bot,
- clear operator instructions for syncing repo changes into the live bot.

---

## 2) Repository Structure and Feature Groups

Top-level feature scripts live under:
- `Actions/`
- `BuildTools/` (local tooling that runs outside Streamer.bot)
- `README.md` (root index + operator workflow)
- Feature READMEs under `Actions/**/README.md` (script references split by feature/subfeature)

The repository uses a **feature → subfeature → scripts** hierarchy:
- `Actions/<Feature Group>/<Subfeature>/script.cs`

Where:
- `<Feature Group>` maps to a Streamer.bot action group / feature area.
- `<Subfeature>` maps to a contained behavior set (mini-game, module, storyline branch, etc.).

Current groups include:
- `Actions/Squad/` (chat interaction + mini-game behavior)
- `Actions/Commanders/` (commander role assignment + commander-specific chat commands)
- `Actions/Twitch Integration/` (stream lifecycle/core integration behavior)
- `Actions/LotAT/` (reserved/in-progress group)

Current `Twitch Integration` subfeatures include:
- `Actions/Twitch Integration/Bits/`

Current `Squad` subfeatures include:
- `Actions/Squad/Clone/`
- `Actions/Squad/Duck/`
- `Actions/Squad/Pedro/`
- `Actions/Squad/Toothless/`

Current `Commanders` subfeatures include:
- `Actions/Commanders/Captain Stretch/`
- `Actions/Commanders/The Director/`
- `Actions/Commanders/Water Wizard/`

`Commanders` behavior model:
- A Twitch user can redeem into one of three commander roles.
- All three commanders can be active concurrently.
- Each commander has their own commander-specific commands/actions.
- Existing redeem/assignment behavior should be preserved unless a change is explicitly requested.

### Conventions
- Keep scripts inside the correct **feature group + subfeature** folder.
- Prefer one folder per mini-game/module (e.g., `Toothless`, `Duck`).
- Use descriptive file names (`<subfeature>-main.cs`, `<subfeature>-resolve.cs`, etc.) when possible.
- Avoid moving scripts between feature groups or subfeatures unless explicitly requested.

### BuildTools Conventions (External Utilities)
- Place terminal-run utilities under `BuildTools/<Integration>/...`.
- Current integration: `BuildTools/MixItUp/get_commands.py`.
- BuildTools scripts are **not** pasted into Streamer.bot actions.
- Prefer Python for local tooling unless otherwise requested.
- For Mix It Up command ingestion, support API pagination (`skip` + `pageSize`) and write operator-readable output to file.

---

## 3) What Agents Should Do

In scope:
- Implement/maintain C# scripts for Streamer.bot actions.
- Implement/maintain local external tooling under `BuildTools/` (e.g., Mix It Up API scripts).
- Make focused, minimal-risk fixes.
- Improve readability and reliability without changing intended behavior.
- Leverage the docs set whenever current code context is needed:
  - root `README.md` for index/workflow,
  - feature/subfeature READMEs under `Actions/**/README.md` for script behavior, purpose, and global variable usage.
- Use thorough, beginner-friendly comments so logic is easy to follow.
- Add or update inline comments where logic is non-obvious.
- Document any manual sync steps needed in Streamer.bot or local terminal workflow.

Out of scope unless explicitly requested:
- Broad refactors across unrelated feature groups.
- Renaming triggers/behavior that chat depends on.
- Changing core stream-start/housekeeping behavior without approval.
- Introducing unnecessary external dependencies (Streamer.bot scripts should stay runtime-compatible; BuildTools should prefer stdlib-first solutions).

---

## 4) Feature Categories and Expectations

### A) Stream Housekeeping / Core Integrations
Examples: startup routines, stream state initialization, Twitch integration glue.

Requirements:
- Favor reliability and idempotency.
- Fail safely (log useful info, avoid cascading failures).
- Preserve existing operator workflow unless asked to change it.
- For external TTS/readout integrations, include queue-safe timing/wait behavior so rapid triggers do not overlap/cut off active readouts.

### B) Chat Mini-games / Interaction Features
Examples: `Squad` scripts, `Commanders` scripts, and similar interactive systems.

Requirements:
- Handle spam/rapid triggers gracefully.
- Use deterministic resolution logic where possible.
- Protect against invalid or missing inputs.
- Preserve fairness and existing game feel unless explicitly adjusted.

### C) CYOA / Narrative Night Systems
Requirements:
- Keep branch/state logic explicit and auditable.
- Support clean reset/restart behavior.
- Ensure moderator/operator overrides are safe and clear.
- Avoid accidental progression from malformed input.

---

## 5) Streamer.bot Compatibility Rules

All scripts must be compatible with copy/paste execution in Streamer.bot:
- No assumptions about traditional build/deploy pipelines.
- Prefer self-contained logic per script when practical.
- Guard against null/empty variables from runtime context.
- Use defensive error handling around external/event data.
- Ensure chat-facing output is safe and intentional.

If a change depends on setup in Streamer.bot UI (variables, trigger wiring, action ordering), include those setup notes in your change summary.

---

## 6) State Management and Safety

When editing features that track state:
- Document where state is read/written (global vars, persisted vars, in-action flow).
- Preserve existing key names unless migration is explicitly requested.
- Consider concurrent triggers and re-entry edge cases.
- For `Commanders`, preserve support for three simultaneously active commander slots unless explicitly changing that model.
- Add reset paths for long-running sessions (especially mini-games/CYOA).

Safety requirements:
- Never expose secrets/tokens in code comments, logs, or chat output.
- Sanitize or validate user input before using it in control flow.
- Keep moderator/admin-only controls protected.

---

## 7) Testing and Validation (Manual-First)

Because runtime is Streamer.bot, validation is primarily manual:

Minimum validation after meaningful changes:
1. Syntax sanity check in script.
2. Paste script into the intended Streamer.bot action.
3. Trigger happy path once.
4. Trigger at least one edge case (missing input, repeated trigger, cooldown path, etc.).
5. Verify logs and chat output for clarity/safety.

For high-impact changes (startup/core, stateful mini-game, CYOA progression), also test rollback/reset behavior.

For external `BuildTools/` scripts, minimum validation after meaningful changes:
1. CLI help/syntax sanity check.
2. Happy-path API call against expected local service (when available).
3. Verify output file is written and readable.
4. Verify at least one edge case (service offline, malformed response, empty result set, pagination boundary).

---

## 8) Sync Workflow (Repo ↔ Streamer.bot)

This repo is the source-of-truth for script text.

When changes are made:
- For Streamer.bot actions: update script file(s) in `Actions/<Feature Group>/<Subfeature>/...`.
- For external tooling: update file(s) in `BuildTools/<Integration>/...`.
- Provide a copy/paste mapping in the summary for Streamer.bot scripts:
  - full file path (including subfeature folder)
  - target Streamer.bot action/group
  - any required UI variable/trigger changes
- For BuildTools changes, provide run instructions (command + expected output file path) instead of paste targets.
- After paste, run smoke tests in Streamer.bot.

Recommended commit note style includes: `synced-to-streamerbot: yes/no`.

---

## 9) Change Control Rules

- Prefer small, targeted edits.
- Preserve existing external behavior unless requested.
- Do not rename files/actions casually when operators rely on them.
- Highlight any breaking change before implementation.
- If requirements are ambiguous for live behavior, ask before proceeding.

---

## 10) Documentation Requirements per Feature Script

Commenting standard (applies to all scripts):
- Always include thorough comments that explain intent and flow.
- Write comments and logic in plain language so a beginner developer can follow along.
- Prefer short, clear steps over dense or clever code.

For non-trivial scripts, include/update short header comments covering:
- Purpose
- Expected trigger/input
- Required runtime variables
- Key outputs/side effects
- Any operator notes (cooldown, reset command, manual setup)

When script behavior changes, update the matching feature README (`Actions/**/README.md`) using the standard sections:
- Purpose
- Expected Trigger / Input
- Required Runtime Variables
- Key Outputs / Side Effects
- Mix It Up Actions
- OBS Interactions
- Wait Behavior
- Chat / Log Output
- Operator Notes

---

## 11) Agent Response Format for Code Changes

When submitting changes, include:
1. **Changed files** (clear paths)
2. **Behavioral summary** (what changed)
3. **Streamer.bot paste targets** (where each file should be pasted, or `N/A` for external BuildTools-only changes)
4. **Manual setup steps** (if any)
5. **Validation checklist executed**

Keep summaries concise and operator-friendly.

---

## 12) Default Priority Order

1. Live stream reliability
2. Safe chat-facing behavior
3. Backward compatibility for existing features
4. Maintainable, readable scripts
5. Minimal operator friction during manual copy/paste sync
