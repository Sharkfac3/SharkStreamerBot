# Prompt 102 — Actions Helper Snippets Split Design

Date: 2026-04-27  
Scope: design only for splitting `Actions/HELPER-SNIPPETS.md`; no helper/code execution in this prompt.

## Overview

Audit finding 1.1 is correct: `Actions/HELPER-SNIPPETS.md` is a high-traffic, 1,043-line copy/paste reference that currently mixes eight unrelated helper domains behind one file. A junior reader must scan mini-game locks, Mix It Up HTTP, chat args, OBS scenes, CPH signatures, timers, and a full JSON parser before finding the relevant pattern.

Recommended target shape:

- Keep reusable Streamer.bot C# snippets under a new `Actions/Helpers/` documentation cluster.
- Split by concept boundary, not by line count.
- Keep `Actions/HELPER-SNIPPETS.md` as a thin compatibility index during/after the split, because many active docs and workflows already point at it.
- Add new helper docs to `.agents/manifest.json` as co-location entries, but do **not** promote each helper file to a separate role/skill unless operator tooling later needs that granularity.

Primary recommendation summary:

| Decision | Recommendation |
|---|---|
| Helper cluster | Create `Actions/Helpers/` with one concept file per helper domain. |
| `Actions/HELPER-SNIPPETS.md` fate | Option A: keep as a thin index and backward-compatible landing page. |
| Mix It Up helper | Keep a Streamer.bot-side helper file in `Actions/Helpers/`, cross-link to `Tools/MixItUp/AGENTS.md` for payload/tooling authority. |
| Mini-game contract checklist | Move to dedicated `Actions/Helpers/mini-game-contract.md`, then link it from `Actions/Squad/AGENTS.md`. |

## Section Inventory

Line ranges are approximate from the current `Actions/HELPER-SNIPPETS.md` line map:

```text
12    ## 1) Mini-game Lock Helper (Global, cross-feature)
83    ## 2) Mix It Up Command API Helper
221   ## 3) Chat Message Input Helper (message/rawInput)
274   ## 4) OBS Scene Switching
314   ## 5) Verified CPH API Method Signatures
381   ## 6) Timer Management
466   ## 7) JSON Parse / Serialize Helper (No External Libraries)
1037  ## Required mini-game contract checklist
```

| Current H2 section | Approx. lines | Concept domain | Proposed destination | Cross-references into section/content | Cross-references out/dependencies |
|---|---:|---|---|---|---|
| `1) Mini-game Lock Helper (Global, cross-feature)` | 12-82 | Shared mini-game lock globals and acquire/release snippets. | `Actions/Helpers/mini-game-lock.md` | Direct implementation use in `Actions/Squad/Clone/clone-empire-main.cs`, `clone-empire-start.cs`, `clone-empire-move.cs`, `clone-empire-tick.cs`, `Actions/Squad/Duck/duck-main.cs`, `duck-call.cs`, `duck-resolve.cs`, `Actions/Squad/Pedro/pedro-main.cs`, `pedro-resolve.cs`, `Actions/Squad/Toothless/toothless-main.cs`. Domain references: `Actions/Squad/AGENTS.md`, `Actions/Twitch Core Integrations/AGENTS.md`, `Actions/SHARED-CONSTANTS.md` mini-game lock section. | Depends on `VAR_MINIGAME_ACTIVE` and `VAR_MINIGAME_NAME` from `Actions/SHARED-CONSTANTS.md`; depends on stream-start reset in `Actions/Twitch Core Integrations/stream-start.cs`; pairs with mini-game contract checklist. |
| `2) Mix It Up Command API Helper` | 83-220 | Streamer.bot C# HTTP helper for Mix It Up command API, optional user-message special identifier convention, unlock pacing. | `Actions/Helpers/mixitup-command-api.md` | Direct/variant use in Commanders (`captain-stretch-redeem.cs`, `captain-stretch-generalfocus.cs`, The Director primary/secondary/redeem, Water Wizard redeem/castrest), Rest Focus Loop scripts, Temporary scripts, Squad unlock scripts, Twitch Bits scripts, Twitch Channel Points scripts, Hype/Core scripts where Mix It Up payload conventions apply. Doc refs: `Actions/Temporary/AGENTS.md`, `Tools/MixItUp/AGENTS.md`, Twitch route guides, Commanders guide. | Depends on `System.Net.Http`, `System.Text`, and `System.Text.Json` for outbound HTTP payloads; depends on Mix It Up API endpoint/payload authority in `Tools/MixItUp/AGENTS.md`; uses constants/wait guidance from `Actions/SHARED-CONSTANTS.md` Bits / Mix It Up Unlock Pacing. |
| `3) Chat Message Input Helper (message/rawInput)` | 221-273 | Defensive trigger argument reading for chat/user text, duplicate msgId guard, sender tuple. | `Actions/Helpers/chat-input.md` | Direct use: `Actions/Squad/Pedro/pedro-call.cs` uses `GetMessageText()`. Conceptual use: Twitch Bits read `messageStripped`, `message`, `rawInput`; Twitch Channel Points text-input reward notes; Core watch-streak optional message handling. Doc refs: `Actions/Twitch Bits Integrations/AGENTS.md`, `Actions/Twitch Channel Points/AGENTS.md`, `Actions/Twitch Core Integrations/AGENTS.md`. | Depends on `CPH.TryGetArg<T>`, `CPH.GetGlobalVar`, `CPH.SetGlobalVar`; should cross-link to `Actions/Helpers/cph-api-signatures.md` for trigger arg signature and common keys. |
| `4) OBS Scene Switching` | 274-313 | Correct OBS scene switch method and mode-aware scene resolution. | `Actions/Helpers/obs-scenes.md` | Direct use: `Actions/Voice Commands/scene-chat.cs`, `scene-main.cs`, `scene-housekeeping.cs`, `scene-dance.cs`; `Actions/Twitch Channel Points/disco-party.cs`. Doc refs: `Actions/Voice Commands/AGENTS.md`, `Actions/Twitch Channel Points/AGENTS.md`. | Depends on `Actions/SHARED-CONSTANTS.md` OBS and Stream Mode sections; duplicates/extends CPH OBS signature in `cph-api-signatures.md`, so it should cross-link there instead of repeating too much. |
| `5) Verified CPH API Method Signatures` | 314-380 | Verified Streamer.bot CPH method signatures and policy for unverified calls. | `Actions/Helpers/cph-api-signatures.md` | Referenced broadly by any C# action work through current `HELPER-SNIPPETS.md` references. Specific known uses: OBS scene scripts, all global var scripts, timer scripts, chat feedback scripts, trigger-arg scripts. `Actions/Rest Focus Loop/AGENTS.md` explicitly mentions verified timer methods; `Actions/Twitch Channel Points/AGENTS.md` explicitly mentions direct OBS methods. | Depends on official Streamer.bot docs URL; cross-links to `timers.md` for timer patterns and `obs-scenes.md` for OBS scene switching examples. |
| `6) Timer Management` | 381-465 | Timer start/stop/reset and interval-update patterns; `SetTimerInterval` verification warning. | `Actions/Helpers/timers.md` | Direct use: `Actions/Rest Focus Loop/*.cs`, `Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs`, `Actions/Commanders/Water Wizard/water-wizard-castrest.cs`, LotAT timer scripts (`lotat-start-main.cs`, `lotat-node-enter.cs`), Temporary focus scripts. Doc refs: `Actions/Rest Focus Loop/AGENTS.md`, `Actions/LotAT/AGENTS.md`, related READMEs. | Depends on timer names in `Actions/SHARED-CONSTANTS.md`; cross-links to `cph-api-signatures.md` for verified/unverified method status. |
| `7) JSON Parse / Serialize Helper (No External Libraries)` | 466-1036 | Large no-external-library JSON parser/serializer for Streamer.bot inline C#; DataContract object mapping. | `Actions/Helpers/json-no-external-libraries.md` | Direct use: most `Actions/LotAT/*.cs`; Clone Empire scripts; overlay payload guidance in `Actions/Overlay/README.md`, `Actions/Overlay/broker-publish.cs`, `Actions/Overlay/AGENTS.md`; `Actions/LotAT/AGENTS.md`. | Depends on `System`, `System.Collections.Generic`, `System.Runtime.Serialization`, `System.Text`; cross-links to `mixitup-command-api.md` because Mix It Up outbound HTTP intentionally still uses `System.Text.Json` for that proven path; cross-links to `Actions/Overlay/AGENTS.md` for broker payloads. |
| `Required mini-game contract checklist` | 1037-1043 | Checklist for lock usage and doc updates when creating/changing mini-games. | `Actions/Helpers/mini-game-contract.md` (recommended) | Doc refs: `Actions/Squad/AGENTS.md` preserves mini-game lock contract; `Actions/Squad/README.md` currently says required mini-game contract lives in root README, which appears stale/misleading and should be corrected during execution. | Depends on `mini-game-lock.md`, `Actions/SHARED-CONSTANTS.md`, and `Actions/Twitch Core Integrations/stream-start.cs` reset behavior. |

### Broad current references to `Actions/HELPER-SNIPPETS.md`

These references should either stay pointed at the thin index or be updated to a specific helper file when the local guide clearly needs one concept:

- Agent/workflow/shared docs: `.agents/_shared/project.md`, `.agents/_shared/conventions.md`, `.agents/roles/streamerbot-dev/role.md`, `.agents/workflows/validation.md`, `.agents/workflows/sync.md`.
- Human docs: `Docs/ONBOARDING.md`, `Docs/Architecture/repo-structure.md`, `Docs/AGENT-WORKFLOW.md`.
- Action route guides: `Actions/Commanders/AGENTS.md`, `Actions/Destroyer/AGENTS.md`, `Actions/Intros/AGENTS.md`, `Actions/LotAT/AGENTS.md`, `Actions/Overlay/AGENTS.md`, `Actions/Rest Focus Loop/AGENTS.md`, `Actions/Squad/AGENTS.md`, `Actions/Temporary/AGENTS.md`, `Actions/Twitch Bits Integrations/AGENTS.md`, `Actions/Twitch Channel Points/AGENTS.md`, `Actions/Twitch Core Integrations/AGENTS.md`, `Actions/Twitch Hype Train/AGENTS.md`, `Actions/Voice Commands/AGENTS.md`, `Actions/XJ Drivethrough/AGENTS.md`.
- Tool docs: `Tools/StreamerBot/AGENTS.md`, `Tools/MixItUp/AGENTS.md`.
- Specific section-ish references: `Actions/Overlay/README.md` and `Actions/Overlay/broker-publish.cs` mention `Actions/HELPER-SNIPPETS.md § 7`; these should point to `Actions/Helpers/json-no-external-libraries.md`.

## Proposed Splits

### 1. `Actions/Helpers/README.md`

Purpose: folder index and routing surface for all helper snippets.

Absorbs source sections:

- None as body content; new index only.

Required frontmatter:

```yaml
---
id: actions-helpers
type: domain-route
description: Index for reusable Streamer.bot C# helper snippets under Actions/Helpers.
owner: streamerbot-dev
secondaryOwners:
  - ops
workflows:
  - change-summary
  - sync
  - validation
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/README.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- Point broad docs that want a generic helper index here or to the compatibility index in `Actions/HELPER-SNIPPETS.md`.
- The `Actions/HELPER-SNIPPETS.md` thin index should point here.

### 2. `Actions/Helpers/mini-game-lock.md`

Absorbs source H2 sections:

- `## 1) Mini-game Lock Helper (Global, cross-feature)`.

Required frontmatter:

```yaml
---
id: actions-helper-mini-game-lock
type: reference
description: Reusable Streamer.bot C# mini-game lock constants and acquire/release snippets.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/mini-game-lock.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Squad/AGENTS.md` should link directly here for the lock contract.
- `Actions/Twitch Core Integrations/AGENTS.md` can link here when discussing stream-start reset for mini-game lock state.
- `Actions/SHARED-CONSTANTS.md` Mini-game Lock section can optionally add a back-link here if execution scope allows.

### 3. `Actions/Helpers/mini-game-contract.md`

Absorbs source H2 sections:

- `## Required mini-game contract checklist`.

Required frontmatter:

```yaml
---
id: actions-helper-mini-game-contract
type: reference
description: Checklist for Streamer.bot mini-game lock usage, terminal paths, and documentation updates.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/mini-game-contract.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Squad/AGENTS.md` should link directly here and preserve its current local mini-game guidance.
- `Actions/Squad/README.md` should fix the stale statement that says the required mini-game contract is in root README; point to this file instead.
- `Actions/Helpers/mini-game-lock.md` should cross-link to this checklist.

Rationale for dedicated file over moving into `Actions/Squad/AGENTS.md`:

- The checklist is about a reusable runtime contract, not agent activation/routing.
- Mini-games are currently Squad-owned, but the lock is explicitly global/cross-feature; a future non-Squad mini-game should not have to load Squad docs just to find the checklist.
- `Actions/Squad/AGENTS.md` can stay a route guide and link to the dedicated contract.

### 4. `Actions/Helpers/mixitup-command-api.md`

Absorbs source H2 sections:

- `## 2) Mix It Up Command API Helper`.

Required frontmatter:

```yaml
---
id: actions-helper-mixitup-command-api
type: reference
description: Streamer.bot C# helper for calling Mix It Up command API and pacing unlock effects.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/mixitup-command-api.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Temporary/AGENTS.md` should point here for the C# helper pattern.
- Twitch guides and Commanders guide can keep broad helper index links unless they mention Mix It Up specifically; targeted links would help `Actions/Twitch Bits Integrations/AGENTS.md`, `Actions/Twitch Hype Train/AGENTS.md`, `Actions/Twitch Core Integrations/AGENTS.md`, and `Actions/Commanders/AGENTS.md`.
- `Tools/MixItUp/AGENTS.md` should keep authority for Mix It Up tooling/payload conventions and link to this file only for the Streamer.bot C# helper and wait snippet.

Boundary decision:

- Keep this file in `Actions/Helpers/`, because the code is a Streamer.bot inline C# helper.
- Cross-link prominently to `Tools/MixItUp/AGENTS.md`, because payload convention, command discovery, and Mix It Up operational tooling belong there.

### 5. `Actions/Helpers/chat-input.md`

Absorbs source H2 sections:

- `## 3) Chat Message Input Helper (message/rawInput)`.

Required frontmatter:

```yaml
---
id: actions-helper-chat-input
type: reference
description: Streamer.bot C# snippets for defensive chat text, duplicate message, and sender argument handling.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/chat-input.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Twitch Bits Integrations/AGENTS.md` and `Actions/Twitch Channel Points/AGENTS.md` can link here for message/rawInput defensive reading.
- `Actions/Squad/AGENTS.md` or Pedro README can link here if Pedro chat parsing is touched in execution.

### 6. `Actions/Helpers/obs-scenes.md`

Absorbs source H2 sections:

- `## 4) OBS Scene Switching`.

Required frontmatter:

```yaml
---
id: actions-helper-obs-scenes
type: reference
description: Streamer.bot C# OBS scene switching helper and direct-call guidance.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/obs-scenes.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Voice Commands/AGENTS.md` should point here for `CPH.ObsSetScene` and mode-aware scene switching.
- `Actions/Twitch Channel Points/AGENTS.md` should point here for Disco Party OBS scene behavior.
- `Actions/SHARED-CONSTANTS.md` OBS / Stream Mode sections can optionally back-link here.

### 7. `Actions/Helpers/cph-api-signatures.md`

Absorbs source H2 sections:

- `## 5) Verified CPH API Method Signatures`.

Required frontmatter:

```yaml
---
id: actions-helper-cph-api-signatures
type: reference
description: Verified Streamer.bot CPH method signatures and policy for unverified API calls.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/cph-api-signatures.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- Broad helper docs can link here when a task is about API method availability.
- `Actions/Rest Focus Loop/AGENTS.md` can point here and `timers.md` for the `SetTimerInterval` verification status.
- `Actions/Twitch Channel Points/AGENTS.md` and `Actions/Voice Commands/AGENTS.md` can point here or `obs-scenes.md` for OBS method signature safety.

### 8. `Actions/Helpers/timers.md`

Absorbs source H2 sections:

- `## 6) Timer Management`.

Required frontmatter:

```yaml
---
id: actions-helper-timers
type: reference
description: Streamer.bot timer enable, disable, reset, interval-update, and verification patterns.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/timers.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Rest Focus Loop/AGENTS.md` should point directly here.
- `Actions/LotAT/AGENTS.md` and `Actions/LotAT/README.md` can link here where they discuss authored commander/dice timer durations and `CPH.SetTimerInterval`.
- Commanders Captain Stretch / Water Wizard READMEs may optionally link here for rest/focus override timer behavior.

### 9. `Actions/Helpers/json-no-external-libraries.md`

Absorbs source H2 sections:

- `## 7) JSON Parse / Serialize Helper (No External Libraries)`.

Required frontmatter:

```yaml
---
id: actions-helper-json-no-external-libraries
type: reference
description: No-external-library JSON parse/serialize helper for Streamer.bot inline C# scripts.
owner: streamerbot-dev
secondaryOwners:
  - lotat-tech
  - app-dev
  - ops
status: active
---
```

Manifest co-location entry:

```json
{
  "path": "Actions/Helpers/json-no-external-libraries.md",
  "owner": "streamerbot-dev",
  "status": "active"
}
```

Cross-reference updates:

- `Actions/Overlay/AGENTS.md`, `Actions/Overlay/README.md`, and `Actions/Overlay/broker-publish.cs` should point directly here instead of `Actions/HELPER-SNIPPETS.md § 7`.
- `Actions/LotAT/AGENTS.md` can link directly here for reusable JSON patterns.
- `Actions/Squad/AGENTS.md` or Clone README can link here if Clone Empire docs currently mention structured global JSON.

## Cross-Reference Update Plan

Execution prompt `102b` should update links in layers so the repo remains navigable after each step.

### Layer 1 — Create helper files and compatibility indexes

1. Create `Actions/Helpers/README.md`.
2. Create one file per helper concept.
3. Replace `Actions/HELPER-SNIPPETS.md` with a thin index preserving its old path.
4. Ensure every old H2 section has a destination link in the index.

### Layer 2 — Update targeted references that mention a specific concept

High-priority targeted updates:

| Current reference | Proposed new target |
|---|---|
| `Actions/Overlay/README.md` mentions `Actions/HELPER-SNIPPETS.md § 7` | `Actions/Helpers/json-no-external-libraries.md` |
| `Actions/Overlay/broker-publish.cs` comments mention `Actions/HELPER-SNIPPETS.md § 7` | `Actions/Helpers/json-no-external-libraries.md` |
| `Actions/Overlay/AGENTS.md` says helper snippets when complex JSON serialization is needed | `Actions/Helpers/json-no-external-libraries.md` |
| `Actions/Rest Focus Loop/AGENTS.md` says verified timer methods/helper patterns | `Actions/Helpers/timers.md` and optionally `cph-api-signatures.md` |
| `Actions/Temporary/AGENTS.md` says Mix It Up API helper pattern | `Actions/Helpers/mixitup-command-api.md` |
| `Actions/Voice Commands/AGENTS.md` discusses direct OBS scene setter | `Actions/Helpers/obs-scenes.md` |
| `Actions/Twitch Channel Points/AGENTS.md` discusses direct OBS methods and no reflection | `Actions/Helpers/obs-scenes.md` |
| `Actions/Squad/README.md` stale mini-game contract note | `Actions/Helpers/mini-game-contract.md` |

### Layer 3 — Keep or update broad helper-index links

For broad references such as `.agents/_shared/project.md`, `.agents/_shared/conventions.md`, `Docs/ONBOARDING.md`, `Docs/Architecture/repo-structure.md`, `Tools/StreamerBot/AGENTS.md`, and `.agents/workflows/sync.md`, two safe options exist:

- Point to `Actions/HELPER-SNIPPETS.md` if it remains the compatibility index.
- Or point to `Actions/Helpers/README.md` if the operator wants the new folder to become the obvious home.

Recommendation: during `102b`, update broad docs to `Actions/Helpers/README.md` only where the sentence naturally says “helper index” or “helper patterns”; keep a short compatibility note in `Actions/HELPER-SNIPPETS.md` for external memory and older prompts.

### Layer 4 — Manifest co-location

Add co-location entries for:

- `Actions/Helpers/README.md`
- `Actions/Helpers/mini-game-lock.md`
- `Actions/Helpers/mini-game-contract.md`
- `Actions/Helpers/mixitup-command-api.md`
- `Actions/Helpers/chat-input.md`
- `Actions/Helpers/obs-scenes.md`
- `Actions/Helpers/cph-api-signatures.md`
- `Actions/Helpers/timers.md`
- `Actions/Helpers/json-no-external-libraries.md`

Do not add all helper files as top-level `skills` unless the validator or operator requires every co-located doc to have a skill entry. Existing manifest shape supports simple `co_locations` with `path`, `owner`, and `status`.

### Estimated execution scope for `102b`

Approximate file count if recommendations are accepted:

- 9 new helper docs under `Actions/Helpers/`.
- 1 rewritten compatibility index: `Actions/HELPER-SNIPPETS.md`.
- 1 manifest update: `.agents/manifest.json`.
- 10-18 cross-reference doc/comment updates, depending on how aggressive the operator wants targeted links to be.
- No runtime `.cs` logic changes required. One `.cs` comment-only update is recommended for `Actions/Overlay/broker-publish.cs` if the operator allows comment/doc updates in code files.

## `Actions/HELPER-SNIPPETS.md` Post-Split Fate

### Option A — Keep as thin index

Keep `Actions/HELPER-SNIPPETS.md` and replace the body with:

- frontmatter if desired/consistent with local docs,
- a short compatibility note,
- a table pointing each old section title to its new `Actions/Helpers/*.md` file,
- a note that snippets are now organized by concept.

Pros:

- Lowest breakage risk for the highest-traffic helper reference.
- Existing prompts, docs, workflows, and agent muscle memory keep landing somewhere useful.
- Allows gradual cross-reference updates without requiring perfect coverage in one pass.
- Keeps `Docs/AGENT-WORKFLOW.md` examples and older migration notes understandable.

Cons:

- Maintains two possible entry points (`HELPER-SNIPPETS.md` and `Actions/Helpers/README.md`).
- Requires a small ongoing convention: old path is compatibility index, new folder is source.

### Option B — Delete entirely; `Actions/Helpers/README.md` becomes the index

Delete `Actions/HELPER-SNIPPETS.md` and make `Actions/Helpers/README.md` the only helper index.

Pros:

- Cleaner final architecture.
- No compatibility landing page to maintain.

Cons:

- Higher link-update blast radius.
- More likely to break old prompts, local memory, and human bookmarks.
- Not necessary to solve the audit issue; a thin index solves findability while preserving continuity.

### Recommendation

Choose **Option A**. Keep `Actions/HELPER-SNIPPETS.md` as a thin compatibility index for at least one migration cycle. This is the safer path because the file is explicitly called out as the highest-traffic reference for `Actions/*.cs` work.

## Open Decisions

Operator should resolve these before spawning `102b-actions-helper-snippets-split-execute`:

1. **Index fate:** approve Option A (thin compatibility index) or require Option B (delete `Actions/HELPER-SNIPPETS.md` and use `Actions/Helpers/README.md` only).
2. **Mix It Up boundary:** approve keeping `Actions/Helpers/mixitup-command-api.md` for the Streamer.bot C# helper while cross-linking `Tools/MixItUp/AGENTS.md` as payload/tooling authority, or move all Mix It Up guidance to `Tools/MixItUp/` and leave only a pointer from Actions.
3. **Mini-game contract location:** approve dedicated `Actions/Helpers/mini-game-contract.md`, or move the checklist into `Actions/Squad/AGENTS.md`.
4. **Manifest granularity:** approve simple co-location entries only, or require helper docs to be represented as manifest skills/domains as well.
5. **Cross-reference aggressiveness:** in `102b`, update only targeted concept-specific refs plus indexes, or also convert broad `HELPER-SNIPPETS.md` references across docs to `Actions/Helpers/README.md`.
6. **Comment-only code references:** allow `102b` to update comments in `.cs` files that mention old helper paths (not runtime logic), especially `Actions/Overlay/broker-publish.cs`, or keep all `.cs` files untouched.

## Handoff

Execution does **not** happen in this prompt. If the operator ratifies the design, spawn a separate `102b-actions-helper-snippets-split-execute` prompt.

Recommended `102b` acceptance checks:

- All eight current H2 sections in `Actions/HELPER-SNIPPETS.md` are represented in new helper docs.
- `Actions/HELPER-SNIPPETS.md` line count drops substantially and remains an index if Option A is approved.
- Targeted cross-references point to specific helper docs where useful.
- `python3 Tools/AgentTree/validate.py` passes.
- If comments in `.cs` files are updated, report them as comment-only and list no Streamer.bot paste targets unless runtime code changes also occur.
