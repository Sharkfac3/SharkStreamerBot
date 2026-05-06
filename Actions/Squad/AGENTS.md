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

Squad hosts four Streamer.bot mini-games and shared Squad interactions for SharkFac3's stream: Clone Empire, Duck Call, Pedro, Toothless, plus offering-token behavior and the `!game` help command.

Use this guide for work under `Actions/Squad/`. Game-specific mechanics live in each subfolder README; do not duplicate or replace those docs here.

## Ownership

`streamerbot-dev` owns Squad C# runtime behavior and Streamer.bot paste readiness. Follow the parent action standards in [Actions/AGENTS.md](../AGENTS.md).

Chain to `app-dev` for overlay payloads or `squad.*` broker topic changes; chain to `brand-steward` for public copy or character/flavor changes.

## Required Reading

Read the docs for the file you will edit:

- [Actions/AGENTS.md](../AGENTS.md) — required parent action standards, validation, sync, and handoff expectations.
- [Actions/Helpers/mini-game-lock.md](../Helpers/mini-game-lock.md) — required global mini-game lock pattern.
- [Actions/Helpers/mini-game-contract.md](../Helpers/mini-game-contract.md) — mini-game runtime checklist.
- The relevant game README from the routing table below.
- [Apps/stream-overlay/](../../Apps/stream-overlay/) when overlay publish behavior changes.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes.

## Subfolder Routing

| Subfolder | Game | README |
|---|---|---|
| Clone/ | Clone Empire (grid survival) | [Clone/README.md](Clone/README.md) |
| Duck/ | Duck Call (quack race) | [Duck/README.md](Duck/README.md) |
| Pedro/ | Pedro event | [Pedro/README.md](Pedro/README.md) |
| Toothless/ | Toothless event | [Toothless/README.md](Toothless/README.md) |

## Shared Scripts

| Script | Purpose |
|---|---|
| `offering.cs` | Shared Squad state management |
| `squad-game-help.cs` | Shared helpers for all Squad games |

`offering.cs` manages shared Squad state and experimental offering-token behavior across the Squad area. Do not promote it into canonical LotAT runtime behavior without `lotat-tech` review.

`squad-game-help.cs` provides shared `!game` help for all four games. Update it when the user-facing game list or help text changes.

## Local Workflow

1. Identify whether the change belongs to Clone, Duck, Pedro, Toothless, `offering.cs`, or `squad-game-help.cs`.
2. Read the relevant subfolder README before changing game-specific mechanics.
3. Preserve the global [mini-game lock](../Helpers/mini-game-lock.md): Squad games must acquire the lock before active play and release it on success, failure, timeout, and fault paths.
4. Preserve the mini-game script pattern used by Squad games: the call/main/resolve pattern means a trigger/call script starts or joins interaction, a `main` script owns core game state, a `resolve` or tick script ends the run, and overlay-publish helpers emit broker messages when integrated.
5. Prefer `userId` as the canonical player key for roster/state entries.
6. Keep shared changes in `offering.cs` and `squad-game-help.cs` compatible with all four games.

## Squad-Specific Notes

- Clone uses join-window and game-tick behavior; lock acquisition starts the run and release happens on terminal tick paths.
- Duck uses a call window; acquire in the main/start path and release in resolve.
- Pedro secret unlocks may fire Mix It Up multiple times per stream by design. Do not add a one-per-stream guard unless explicitly requested.
- Toothless rolls resolve immediately; preserve rarity-state tracking and OBS source behavior.
- Duck, Pedro, and Toothless have overlay publish reference templates in their folders. These are not standalone deployed Streamer.bot actions; copy the publish methods into target scripts only when integrating.
- Published broker topics currently include `squad.clone.*`, `squad.duck.*`, `squad.pedro.*`, and `squad.toothless.*`; coordinate app-side before changing topic names or payload shapes.

## Boundaries

For universal C# rules, shared constants, validation, sync, and handoff format, follow [Actions/AGENTS.md](../AGENTS.md).

Squad-specific boundaries:

- Do not edit subfolder READMEs unless explicitly asked; they are the game mechanics source of truth.
- Do not rewrite multiple games when a targeted fix is sufficient.
- Do not rename chat commands, timers, broker topics, global variables, or OBS sources unless explicitly requested.
- Do not treat Pedro repeated secret-unlock fires as a bug.
- Do not change overlay protocol contracts without `app-dev` review.

## Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for validation, sync, paste-target, and change-summary requirements.

Paste targets are the edited `.cs` files under `Actions/Squad/`. Operator must manually paste changed script contents into matching Streamer.bot actions and verify trigger wiring for chat commands, timers, and overlay-publish integration.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "Clone/clone-empire-join.cs",
      "action": "Clone Empire Join",
      "purpose": "Contracts expected runtime behavior for Clone/clone-empire-join.cs.",
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
        "Runs the documented Clone/clone-empire-join.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Clone/clone-empire-join.cs"
    },
    {
      "script": "Clone/clone-empire-main.cs",
      "action": "Clone Empire Main",
      "purpose": "Contracts expected runtime behavior for Clone/clone-empire-main.cs.",
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
        "Runs the documented Clone/clone-empire-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Clone/clone-empire-main.cs"
    },
    {
      "script": "Clone/clone-empire-move.cs",
      "action": "Clone Empire Move",
      "purpose": "Contracts expected runtime behavior for Clone/clone-empire-move.cs.",
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
        "Runs the documented Clone/clone-empire-move.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Clone/clone-empire-move.cs"
    },
    {
      "script": "Clone/clone-empire-start.cs",
      "action": "Clone Empire Start",
      "purpose": "Contracts expected runtime behavior for Clone/clone-empire-start.cs.",
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
        "Runs the documented Clone/clone-empire-start.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Clone/clone-empire-start.cs"
    },
    {
      "script": "Clone/clone-empire-tick.cs",
      "action": "Clone Empire Tick",
      "purpose": "Contracts expected runtime behavior for Clone/clone-empire-tick.cs.",
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
        "Runs the documented Clone/clone-empire-tick.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Clone/clone-empire-tick.cs"
    },
    {
      "script": "Duck/duck-call.cs",
      "action": "Duck Call",
      "purpose": "Contracts expected runtime behavior for Duck/duck-call.cs.",
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
        "Runs the documented Duck/duck-call.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Duck/duck-call.cs"
    },
    {
      "script": "Duck/duck-main.cs",
      "action": "Duck Main",
      "purpose": "Contracts expected runtime behavior for Duck/duck-main.cs.",
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
        "Runs the documented Duck/duck-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Duck/duck-main.cs"
    },
    {
      "script": "Duck/duck-resolve.cs",
      "action": "Duck Resolve",
      "purpose": "Contracts expected runtime behavior for Duck/duck-resolve.cs.",
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
        "Runs the documented Duck/duck-resolve.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Duck/duck-resolve.cs"
    },
    {
      "script": "Duck/overlay-publish.cs",
      "action": "Overlay Publish",
      "purpose": "Contracts expected runtime behavior for Duck/overlay-publish.cs.",
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
        "Runs the documented Duck/overlay-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Duck/overlay-publish.cs"
    },
    {
      "script": "Pedro/overlay-publish.cs",
      "action": "Overlay Publish",
      "purpose": "Contracts expected runtime behavior for Pedro/overlay-publish.cs.",
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
        "Runs the documented Pedro/overlay-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Pedro/overlay-publish.cs"
    },
    {
      "script": "Pedro/pedro-call.cs",
      "action": "Pedro Call",
      "purpose": "Contracts expected runtime behavior for Pedro/pedro-call.cs.",
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
        "Runs the documented Pedro/pedro-call.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Pedro/pedro-call.cs"
    },
    {
      "script": "Pedro/pedro-main.cs",
      "action": "Pedro Main",
      "purpose": "Contracts expected runtime behavior for Pedro/pedro-main.cs.",
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
        "Runs the documented Pedro/pedro-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Pedro/pedro-main.cs"
    },
    {
      "script": "Pedro/pedro-resolve.cs",
      "action": "Pedro Resolve",
      "purpose": "Contracts expected runtime behavior for Pedro/pedro-resolve.cs.",
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
        "Runs the documented Pedro/pedro-resolve.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Pedro/pedro-resolve.cs"
    },
    {
      "script": "Toothless/overlay-publish.cs",
      "action": "Overlay Publish",
      "purpose": "Contracts expected runtime behavior for Toothless/overlay-publish.cs.",
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
        "Runs the documented Toothless/overlay-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Toothless/overlay-publish.cs"
    },
    {
      "script": "Toothless/toothless-main.cs",
      "action": "Toothless Main",
      "purpose": "Contracts expected runtime behavior for Toothless/toothless-main.cs.",
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
        "Runs the documented Toothless/toothless-main.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for Toothless/toothless-main.cs"
    },
    {
      "script": "offering.cs",
      "action": "Offering",
      "purpose": "Contracts expected runtime behavior for offering.cs.",
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
        "Runs the documented offering.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for offering.cs"
    },
    {
      "script": "squad-game-help.cs",
      "action": "Squad Game Help",
      "purpose": "Contracts expected runtime behavior for squad-game-help.cs.",
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
        "Runs the documented squad-game-help.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for squad-game-help.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
