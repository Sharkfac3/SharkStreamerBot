---
id: actions-rest-focus-loop
type: domain-route
description: Rest/focus loop Streamer.bot timer actions, commander integration points, setup, and validation notes.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Rest Focus Loop — Agent Guide

## Purpose

This folder owns the repeating rest/focus loop controller for Streamer.bot. It alternates pre-rest, rest, pre-focus, and focus phases with Streamer.bot timers and Mix It Up alerts.

The key sequence is: `rest-focus-loop-start.cs` starts the loop → timer fires `rest-focus-pre-focus-end.cs` → `rest-focus-focus-end.cs` → timer fires `rest-focus-pre-rest-end.cs` → `rest-focus-rest-end.cs` → loops back.

> Note: commander override scripts in `Actions/Commanders/` may shorten the pre-rest/pre-focus windows by starting rest/focus early; see related guides below.

## Ownership

`streamerbot-dev` owns runtime behavior here. Shared ownership, validation, paste/sync, and brand-review rules inherit from [Actions/AGENTS.md](../AGENTS.md).

## Required Reading

- [Actions/AGENTS.md](../AGENTS.md) for shared constants, helper-pattern routing, contracts, validation, and handoff rules.
- [Actions/Helpers/timers.md](../Helpers/timers.md) and [Actions/Helpers/cph-api-signatures.md](../Helpers/cph-api-signatures.md) for timer API notes.
- [Actions/Commanders/Water Wizard/water-wizard-castrest.cs](../Commanders/Water%20Wizard/water-wizard-castrest.cs) when changing rest override behavior.
- [Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs](../Commanders/Captain%20Stretch/captain-stretch-generalfocus.cs) when changing focus override behavior.
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) for stream-start timer reset behavior.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

- Preserve phase guards: timer-end scripts only advance when `rest_focus_loop_active` is true and `rest_focus_loop_phase` matches the expected phase.
- Preserve fail-closed timer behavior: set the next phase before arming timers; if arming fails, disable all loop timers and clear active state.
- Review commander override scripts whenever durations or timer behavior changes.
- `CPH.SetTimerInterval(string, int)` is a production dependency but remains worth smoke-testing in the operator's Streamer.bot build.

## Runtime Map

Required timers:

| Timer | Expected action |
|---|---|
| `Rest Focus - Pre Rest` | `rest-focus-pre-rest-end.cs` |
| `Rest Focus - Rest` | `rest-focus-rest-end.cs` |
| `Rest Focus - Pre Focus` | `rest-focus-pre-focus-end.cs` |
| `Rest Focus - Focus` | `rest-focus-focus-end.cs` |

Core globals: `rest_focus_loop_active`; `rest_focus_loop_phase` (`idle`, `pre_rest`, `rest`, `pre_focus`, `focus`).

## Script Summary

| Script | Phase handled |
|---|---|
| `rest-focus-loop-start.cs` | Starts/restarts the loop. |
| `rest-focus-pre-focus-end.cs` | Handles the `pre_focus` timer firing and enters `focus`. |
| `rest-focus-focus-end.cs` | Handles `focus` ending and enters `pre_rest`. |
| `rest-focus-pre-rest-end.cs` | Handles the `pre_rest` timer firing and enters `rest`. |
| `rest-focus-rest-end.cs` | Handles `rest` ending and loops toward `pre_focus`. |

## Validation / Boundaries / Handoff

Follow [Actions/AGENTS.md](../AGENTS.md) for shared boundaries, action-script validation, sync, paste targets, and terminal handoff. Keep commander override behavior in `Actions/Commanders/`, do not rename timers/globals/phases without operator approval, and route public wording changes through `brand-steward`.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "rest-focus-focus-end.cs",
      "action": "Rest Focus Focus End",
      "purpose": "Contracts expected runtime behavior for rest-focus-focus-end.cs.",
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
        "Runs the documented rest-focus-focus-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for rest-focus-focus-end.cs"
    },
    {
      "script": "rest-focus-loop-start.cs",
      "action": "Rest Focus Loop Start",
      "purpose": "Contracts expected runtime behavior for rest-focus-loop-start.cs.",
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
        "Runs the documented rest-focus-loop-start.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for rest-focus-loop-start.cs"
    },
    {
      "script": "rest-focus-pre-focus-end.cs",
      "action": "Rest Focus Pre Focus End",
      "purpose": "Contracts expected runtime behavior for rest-focus-pre-focus-end.cs.",
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
        "Runs the documented rest-focus-pre-focus-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for rest-focus-pre-focus-end.cs"
    },
    {
      "script": "rest-focus-pre-rest-end.cs",
      "action": "Rest Focus Pre Rest End",
      "purpose": "Contracts expected runtime behavior for rest-focus-pre-rest-end.cs.",
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
        "Runs the documented rest-focus-pre-rest-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for rest-focus-pre-rest-end.cs"
    },
    {
      "script": "rest-focus-rest-end.cs",
      "action": "Rest Focus Rest End",
      "purpose": "Contracts expected runtime behavior for rest-focus-rest-end.cs.",
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
        "Runs the documented rest-focus-rest-end.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for rest-focus-rest-end.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
