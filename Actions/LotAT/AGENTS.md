---
id: actions-lotat
type: domain-route
description: LotAT Streamer.bot runtime engine scripts, session lifecycle, voting, timers, and paste/sync guidance.
owner: lotat-tech
secondaryOwners:
  - streamerbot-dev
  - lotat-writer
  - app-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Actions/LotAT — Agent Guide

## Purpose

[Actions/LotAT/](./) contains the Streamer.bot C# runtime engine for **Legends of the ASCII Temple (LotAT) v1**, a multi-phase turn-based chat game.

This folder owns live session execution only: starting a run, loading the current story JSON, opening/closing phase windows, tracking roster/votes, entering nodes, resolving runtime mechanics, ending safely, and publishing state to the overlay.

Story content stays in [Creative/WorldBuilding/](../../Creative/WorldBuilding/). Story review/loading tooling stays in [Tools/LotAT/](../../Tools/LotAT/). Overlay rendering stays in [Apps/stream-overlay/](../../Apps/stream-overlay/).

## Ownership

Primary owner: `lotat-tech`. For shared Streamer.bot action ownership, validation, sync, boundaries, and handoff expectations, also follow [Actions/AGENTS.md](../AGENTS.md).

## When to Activate

Use this guide when working on:

- any C# script in [Actions/LotAT/](./)
- LotAT runtime globals, timers, stage transitions, voting, or teardown behavior
- Streamer.bot trigger/action ordering for LotAT scripts
- runtime-side LotAT overlay publishing from C#
- implementation changes required by story schema or command-contract updates

Also load this guide when another domain changes how the live LotAT engine consumes, starts, or advances stories.

## Required Reading

Read these first for LotAT runtime work:

1. [Actions/AGENTS.md](../AGENTS.md) — parent Streamer.bot action rules, shared constants/helper routing, validation, sync, and handoff expectations.
2. [Actions/constants/lotat.md](../constants/lotat.md) — canonical LotAT runtime globals and timer names.
3. [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) — shared LotAT contract across runtime, tooling, story, and overlay domains.
4. [runtime-contract.md](./runtime-contract.md) — runtime globals, timers, state variables, commands, story-file contract, flow, and v1 boundaries.
5. [operator-setup.md](./operator-setup.md) — Streamer.bot timer/trigger setup and live-test checklist.
6. [implementation-map.md](./implementation-map.md) — full script dependency map, trigger/input expectations, and paste/sync notes.
7. [Actions/constants/overlay-broker.md](../constants/overlay-broker.md) — canonical `lotat.*` broker topic names used by the runtime publisher.
8. [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) — story pipeline/tooling responsibilities when runtime changes affect story contracts.
9. [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) and [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) before changing overlay payloads or topic names.

## Local Workflow

1. Confirm the task is runtime implementation, not story authoring, story tooling, or overlay rendering.
2. Preserve the runtime/story boundary: C# actions and Streamer.bot globals hold runtime session state; authored story data belongs in JSON and the creative/story pipeline.
3. Keep start-time runtime checks minimal-safe. Full schema/graph validation belongs upstream in the story pipeline and reviewer tooling.
4. Treat the normal phase sequence as load-bearing: **join → dice roll → decision → node entry → (timeout handlers at each phase) → end session**.
5. If changing story command behavior, coordinate with [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) and update the authoritative story contract before treating the engine change as complete.
6. If changing overlay publishing, update [overlay-publish.cs](./overlay-publish.cs) and verify the app-side protocol/topic definitions.

## Runtime Phase Map

For the full script dependency map, trigger expectations, paste targets, and gotchas, use [implementation-map.md](./implementation-map.md). For state variable definitions, use [runtime-contract.md](./runtime-contract.md).

| Phase | Script | Notes |
|---|---|---|
| Start | `lotat-start-main.cs` | Game entry point |
| Join | `lotat-join.cs` | Player registration |
| Join timeout | `lotat-join-timeout.cs` | Closes join window |
| Dice roll | `lotat-dice-roll.cs` | Rolls for active players |
| Dice timeout | `lotat-dice-timeout.cs` | Resolves on timer |
| Decision input | `lotat-decision-input.cs` | Collects player decisions |
| Decision resolve | `lotat-decision-resolve.cs` | Applies decisions |
| Decision timeout | `lotat-decision-timeout.cs` | Resolves on timer |
| Commander input | `lotat-commander-input.cs` | Commander action input (JSON parser) |
| Commander timeout | `lotat-commander-timeout.cs` | Resolves on timer |
| Node enter | `lotat-node-enter.cs` | Applies node effects |
| End session | `lotat-end-session.cs` | Tears down game state |
| Overlay | `overlay-publish.cs` | Publishes state to overlay |

## LotAT-Specific Notes

- `overlay-publish.cs` is the runtime publisher that sends LotAT state to the overlay; app-side rendering lives outside this folder.
- `lotat-commander-input.cs` contains the canonical hand-rolled JSON parser example for no-external-library Streamer.bot C#; see [Actions/Helpers/json-no-external-libraries.md](../Helpers/json-no-external-libraries.md).
- The loaded runtime story is [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json).
- V1 intentionally excludes offering integration, boost-state integration, late join/leave flow, operator force-close/manual-advance tools, and full runtime schema validation.

## Known Gotchas

- Runtime identity uses lowercase username/login strings, not user IDs, for roster, votes, and commander-target comparison.
- The roster freezes after the join window closes; late join/leave flow is not part of v1.
- Latest valid vote replaces prior valid vote from the same joined user while a decision window remains open.
- If every joined participant has a valid vote, early-close may resolve immediately.
- Zero joiners end the session cleanly before story play.
- Zero valid votes on a node ends the run unresolved; do not invent fallback winners.
- Tie-breaks resolve to the earliest matching choice in the authored `choices` order.
- Commander and dice outcomes are narrative-only in v1; they do not change chaos, branching, vote eligibility, or vote resolution.
- `Actions/Squad/offering.cs` and offering globals are legacy/provisional experimentation, not LotAT v1 mechanics.

## Validation, Boundaries, and Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared validation, sync, boundary, paste-target, and final handoff requirements. For LotAT C# script changes, include exact Streamer.bot paste targets, timer/trigger changes, new or changed globals, story-contract impacts, overlay protocol impacts, and validation output.

## Action Contracts

Contracts for all LotAT scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the `.cs` file.
