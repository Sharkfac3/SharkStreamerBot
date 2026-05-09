---
id: actions-contract-schema
type: schema
description: Action contract format specification and validation instructions for Actions/ scripts.
owner: ops
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# Actions — Contract Schema

This file defines the required format for action contracts across all `Actions/` scripts. Contracts live in each folder's `contracts.md` file. This file tells you what a valid contract looks like and how to validate one.

## Action Contracts

Action-group `contracts.md` files may include a required machine-readable source-of-truth block for scripts in that folder:

````md
<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "example.cs",
      "action": "Streamer.bot action name",
      "purpose": "What this action is supposed to do.",
      "triggers": ["Twitch -> Chat Message"],
      "globals": ["exampleGlobal"],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": ["Step-by-step required runtime behavior."],
      "failureBehavior": ["Required safe failure behavior."],
      "pasteTarget": "Matching Streamer.bot Execute C# Code action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
````

Contract rules:

1. Read the nearest local `AGENTS.md` before editing an action script.
2. Add or update the contract before changing behavior, trigger expectations, globals, timers, OBS names, Mix It Up IDs, overlay topics, service URLs, paste targets, or failure behavior.
3. Run `python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/<folder>/<script>.cs" --stamp` after contract updates to refresh the script stamp.
4. Run the same command without `--stamp` as validation, or run `python3 Tools/StreamerBot/Validation/action_contracts.py --changed` before handoff.
5. Do not treat script comments or implementation as the source of truth when they conflict with the local action contract; fix the contract or fix the script so they align.

## Validation and Handoff

After editing any Actions/**/*.cs file:
1. Run `python Tools/StreamerBot/Validation/action_contracts.py --changed` to check contract alignment.
2. Include Streamer.bot paste targets in your handoff.
3. Note smoke-test steps for the changed action.

After editing an ACTION-CONTRACTS block in any `contracts.md` file:
1. Run `python Tools/StreamerBot/Validation/action_contracts.py --stamp` to refresh SHA256 stamps in .cs files.
2. Run `--all` to confirm clean state.

## Sync and Handoff Expectations

For changed C# files, include in your final summary:

- Streamer.bot action name or likely paste target.
- Trigger expectations, if changed or newly added.
- Globals, timers, OBS sources, broker topics, or Mix It Up command IDs touched.
- Validation performed, such as script review, local grep checks, or smoke-test recommendations.
- Any required operator setup in Streamer.bot, OBS, Mix It Up, or local apps.

If a change affects shared names, update `Actions/SHARED-CONSTANTS.md` and all listed consumers before handoff.
