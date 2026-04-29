---
id: actions-squad
type: domain-route
description: Streamer.bot Squad mini-game actions, overlay handoffs, state variables, paste targets, and validation.
owner: streamerbot-dev
secondaryOwners:
  - app-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Squad — Agent Guide

## Purpose

This folder owns Streamer.bot C# actions for Squad mini-games and interactions: Clone, Duck, Pedro, Toothless, the `!game` help command, and offering-token behavior.

Squad work prioritizes reliable chat-triggered interactions, fair mini-game state handling, safe global-variable use, and compatibility with overlay rendering where present.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Squad/](./), including:

- [Actions/Squad/Clone/](Clone/) scripts
- [Actions/Squad/Duck/](Duck/) scripts
- [Actions/Squad/Pedro/](Pedro/) scripts
- [Actions/Squad/Toothless/](Toothless/) scripts
- [Actions/Squad/offering.cs](offering.cs)
- [Actions/Squad/squad-game-help.cs](squad-game-help.cs)
- README or operator documentation in this folder

Activate `app-dev` when a change alters overlay publish payloads, broker topics, rendering contracts, or copied overlay-publish templates.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, mini-game lock use, Streamer.bot paste readiness, chat trigger expectations, global variable contracts, and Streamer.bot-side overlay publishing calls.

## Secondary Owners / Chain To

- `app-dev` — chain for overlay rendering, `squad.*` broker message payloads, or app-side protocol changes.
- `lotat-tech` — chain if offering behavior is being promoted into approved LotAT runtime mechanics instead of remaining separate experimental offering work.
- `brand-steward` — chain for public-facing help text, flavor text, character interpretation, or chat copy changes.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the specific game README before editing scripts:

- [Actions/Squad/Clone/README.md](Clone/README.md)
- [Actions/Squad/Duck/README.md](Duck/README.md)
- [Actions/Squad/Pedro/README.md](Pedro/README.md)
- [Actions/Squad/Toothless/README.md](Toothless/README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md)
- [Actions/Helpers/mini-game-lock.md](../Helpers/mini-game-lock.md) and [Actions/Helpers/mini-game-contract.md](../Helpers/mini-game-contract.md)
- [Apps/stream-overlay/](../../Apps/stream-overlay/) when overlay publish behavior changes
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify the mini-game or interaction: Clone, Duck, Pedro, Toothless, Offering, or shared help.
2. Preserve the [mini-game lock](../Helpers/mini-game-lock.md) and [mini-game contract checklist](../Helpers/mini-game-contract.md). Avoid overlapping game runs unless the existing game intentionally allows repeated triggers.
3. Prefer `userId` as the canonical player key. It is stable when display names change.
4. Read inputs defensively from Streamer.bot trigger args and global variables.
5. Keep scripts self-contained and paste-ready. Do not assume shared runtime files can be imported by Streamer.bot actions.
6. Update the Script Reference section in this file when trigger variables, state variables, timers, overlay publishing, or operator wiring changes.
7. If adding a new Squad global variable, add/reset it in stream-start behavior and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope; otherwise flag the required follow-up in the handoff.

Squad script map:

| Area | Scripts |
|---|---|
| Clone | [clone-empire-main.cs](Clone/clone-empire-main.cs), [clone-empire-join.cs](Clone/clone-empire-join.cs), [clone-empire-start.cs](Clone/clone-empire-start.cs), [clone-empire-move.cs](Clone/clone-empire-move.cs), [clone-empire-tick.cs](Clone/clone-empire-tick.cs) |
| Duck | [duck-main.cs](Duck/duck-main.cs), [duck-call.cs](Duck/duck-call.cs), [duck-resolve.cs](Duck/duck-resolve.cs) |
| Pedro | [pedro-main.cs](Pedro/pedro-main.cs), [pedro-call.cs](Pedro/pedro-call.cs), [pedro-resolve.cs](Pedro/pedro-resolve.cs) |
| Toothless | [toothless-main.cs](Toothless/toothless-main.cs) |
| Shared | [squad-game-help.cs](squad-game-help.cs), [offering.cs](offering.cs) |

Game-specific notes:

- Clone uses `Clone - Join Window` and `Clone - Game Tick` timers. Acquire the lock in [clone-empire-main.cs](Clone/clone-empire-main.cs) and release it in [clone-empire-tick.cs](Clone/clone-empire-tick.cs) on terminal paths.
- Duck uses the Duck Call Window timer. Acquire the lock in [duck-main.cs](Duck/duck-main.cs) and release it in [duck-resolve.cs](Duck/duck-resolve.cs).
- Pedro secret unlocks may fire Mix It Up multiple times per stream by design. Do not add a one-per-stream guard unless explicitly requested.
- Toothless rolls are instant rarity outcomes. Preserve rarity-state tracking and OBS source behavior.
- Offering is separate experimental offering work. Do not infer approved LotAT runtime contracts from [offering.cs](offering.cs) alone.

Overlay publishing:

- Duck, Pedro, and Toothless have overlay publish reference templates in their game folders.
- These templates are not standalone deployed Streamer.bot actions. Copy relevant publish methods into the target game scripts at integration time.
- Published broker topics include `squad.duck.start`, `squad.duck.update`, `squad.duck.end`, `squad.pedro.start`, `squad.pedro.update`, `squad.pedro.end`, `squad.clone.start`, `squad.clone.update`, `squad.clone.end`, `squad.toothless.start`, and `squad.toothless.end`.
- Toothless has no update topic because rolls resolve immediately.

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point per action, no external runtime imports, and no dependency on repo-only helper files.
- Verify global names, OBS names, timers, and Mix It Up command names against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Verify mini-game lock acquire/release behavior on all success, failure, timeout, and fault paths.
- If overlay payloads change, coordinate with app-side overlay validation under [Apps/stream-overlay/](../../Apps/stream-overlay/).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not rewrite multiple games when a targeted fix is sufficient.
- Do not rename chat commands, timers, broker topics, global variables, or OBS sources unless explicitly requested.
- Do not treat Pedro repeated secret-unlock fires as a bug.
- Do not promote [offering.cs](offering.cs) into canonical LotAT runtime behavior without `lotat-tech` review.
- Do not change overlay protocol contracts without `app-dev` review.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Squad/](./). Operator must manually paste changed script contents into the matching Streamer.bot actions and verify trigger wiring for chat commands, timers, and any overlay-publish integration.

App handoff triggers: any change to overlay publish methods, `squad.*` topics, payload shapes, asset names expected by the overlay, broker connection behavior, or visual timing assumptions.

Brand handoff triggers: public game help, flavor text, character metaphor shifts, community-facing command wording, or messages intended to become stream catchphrases.

---

## Script Reference

### Overlay Integration
Duck, Pedro, and Toothless each have an `overlay-publish.cs` reference template in their folder. These are **not standalone deployed actions** — copy the `Publish*` methods into the target game scripts at integration time. See each game's README for the integration map.

### Shared Constants
- Cross-script key/timer/OBS sync reference: `Actions/SHARED-CONSTANTS.md`

### Helper Snippets
- Reusable copy/paste patterns: `Actions/HELPER-SNIPPETS.md` (compatibility index) and `Actions/Helpers/` (concept-specific helper index)
- Required mini-game contract: `Actions/Helpers/mini-game-contract.md`

---

### Script: `squad-game-help.cs`

#### Purpose
Handles the `!game` command. `!game` lists all available Squad mini-games in chat; `!game <name>` explains the rules of the named mini-game.

#### Expected Trigger / Input
- Chat command wired to `!game`.
- Reads `user`, `input0` (first word after command).

#### Required Runtime Variables
- None (read-only; no globals written).

#### Key Outputs / Side Effects
- Sends one chat message: either the full game list or the rules for the named game.

#### Mix It Up Actions
- None.

#### OBS Interactions
- None.

#### Operator Notes
- Wire to the `!game` chat command trigger.
- Add new games to the `helpMessages` dictionary inside the script as they are built.

---

### Script: `offering.cs`

#### Purpose
Handles offering tokens and applies boost changes, including legacy / experimental LotAT-linked steal behavior.

#### Expected Trigger / Input
- Chat/user input that provides an offering token/member target.

#### Required Runtime Variables
- Reads `lotat_active`.
- Reads/writes `lotat_announcement_sent` (one-time announcement latch).
- Reads `lotat_offering_steal_chance` (clamped 0..100).
- Reads `lotat_steal_multiplier` (minimum 1).
- Reads/writes `boost_<member>_<userId>` (clamped final value 0..30).

#### Key Outputs / Side Effects
- Adjusts per-user per-member boost values.
- Applies LotAT steal modifier when active.

#### Mix It Up Actions
- None.

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends one-time LotAT active announcement.
- Sends unknown-token flavor message when token is invalid.
- Sends result message with delta and resulting boost.

#### Operator Notes
- Keep token/member naming aligned with existing chat commands.
- Preserve boost key format (`boost_<member>_<userId>`) for compatibility.
- For current LotAT v1 planning, treat this script as **separate experimental offering work**, not as an approved LotAT runtime mechanic.
- Do not infer LotAT story/runtime contract rules from this script alone.

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Chat Message (primary trigger for all Squad mini-games)

Squad games are triggered via Twitch → Chat → Message (or a Command trigger for `!` commands).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user who sent the message |
| `userId` | string | Twitch user ID — canonical player identifier in all Squad games |
| `message` | string | Full chat message text |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection (Pedro uses this) |
| `input0` | string | First word after the command trigger (if using a Command trigger) |

> `userId` is the preferred player key — stable even if a user changes their display name. Clone, Duck, Pedro, and Offering all key roster/state entries on `userId`.
