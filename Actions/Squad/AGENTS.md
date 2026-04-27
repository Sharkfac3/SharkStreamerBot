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

Read the folder README and the specific game README before editing scripts:

- [Actions/Squad/README.md](README.md)
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
6. Update the relevant README when trigger variables, state variables, timers, overlay publishing, or operator wiring changes.
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
