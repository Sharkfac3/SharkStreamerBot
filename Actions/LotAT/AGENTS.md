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
2. [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) — shared LotAT contract across runtime, tooling, story, and overlay domains.
3. [runtime-contract.md](./runtime-contract.md) — runtime globals, timers, state variables, commands, story-file contract, flow, and v1 boundaries.
4. [operator-setup.md](./operator-setup.md) — Streamer.bot timer/trigger setup and live-test checklist.
5. [implementation-map.md](./implementation-map.md) — full script dependency map, trigger/input expectations, and paste/sync notes.
6. [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) — story pipeline/tooling responsibilities when runtime changes affect story contracts.
7. [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) and [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) before changing overlay payloads or topic names.

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

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "lotat-commander-input.cs",
      "action": "Lotat Commander Input",
      "purpose": "Contracts expected runtime behavior for lotat-commander-input.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-commander-input.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-commander-input.cs"
    },
    {
      "script": "lotat-commander-timeout.cs",
      "action": "Lotat Commander Timeout",
      "purpose": "Contracts expected runtime behavior for lotat-commander-timeout.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-commander-timeout.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-commander-timeout.cs"
    },
    {
      "script": "lotat-decision-input.cs",
      "action": "Lotat Decision Input",
      "purpose": "Contracts expected runtime behavior for lotat-decision-input.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-decision-input.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-decision-input.cs"
    },
    {
      "script": "lotat-decision-resolve.cs",
      "action": "Lotat Decision Resolve",
      "purpose": "Contracts expected runtime behavior for lotat-decision-resolve.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-decision-resolve.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-decision-resolve.cs"
    },
    {
      "script": "lotat-decision-timeout.cs",
      "action": "Lotat Decision Timeout",
      "purpose": "Contracts expected runtime behavior for lotat-decision-timeout.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-decision-timeout.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-decision-timeout.cs"
    },
    {
      "script": "lotat-dice-roll.cs",
      "action": "Lotat Dice Roll",
      "purpose": "Contracts expected runtime behavior for lotat-dice-roll.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-dice-roll.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-dice-roll.cs"
    },
    {
      "script": "lotat-dice-timeout.cs",
      "action": "Lotat Dice Timeout",
      "purpose": "Contracts expected runtime behavior for lotat-dice-timeout.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-dice-timeout.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-dice-timeout.cs"
    },
    {
      "script": "lotat-end-session.cs",
      "action": "Lotat End Session",
      "purpose": "Contracts expected runtime behavior for lotat-end-session.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-end-session.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-end-session.cs"
    },
    {
      "script": "lotat-join-timeout.cs",
      "action": "Lotat Join Timeout",
      "purpose": "Contracts expected runtime behavior for lotat-join-timeout.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-join-timeout.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-join-timeout.cs"
    },
    {
      "script": "lotat-join.cs",
      "action": "Lotat Join",
      "purpose": "Contracts expected runtime behavior for lotat-join.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-join.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-join.cs"
    },
    {
      "script": "lotat-node-enter.cs",
      "action": "Lotat Node Enter",
      "purpose": "Contracts expected runtime behavior for lotat-node-enter.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-node-enter.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-node-enter.cs"
    },
    {
      "script": "lotat-start-main.cs",
      "action": "Lotat Start Main",
      "purpose": "Contracts expected runtime behavior for lotat-start-main.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented lotat-start-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for lotat-start-main.cs"
    },
    {
      "script": "overlay-publish.cs",
      "action": "Overlay Publish",
      "purpose": "Contracts expected runtime behavior for overlay-publish.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented overlay-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for overlay-publish.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
