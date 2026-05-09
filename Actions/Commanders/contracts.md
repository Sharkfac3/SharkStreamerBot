---
id: commanders-contracts
type: contracts
description: Action contracts for Commanders scripts.
owner: streamerbot-dev
secondaryOwners: [brand-steward]
parent: AGENTS.md
---

# Commanders — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "commanders.cs",
      "action": "Commanders - Active Commanders",
      "purpose": "Responds to the !commanders chat command with the currently active Captain Stretch, The Director, and Water Wizard slot holders.",
      "triggers": [
        "Twitch -> Chat Command -> !commanders"
      ],
      "globals": [
        "current_captain_stretch",
        "current_the_director",
        "current_water_wizard"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [
        "Captain Stretch",
        "The Director",
        "Water Wizard",
        "!commanderhelp"
      ],
      "runtimeBehavior": [
        "Reads all commander slot globals defensively.",
        "Sends active slot holders with Twitch @mentions.",
        "Sends open-deck fallback when all slots are blank.",
        "Does not create, mutate, or persist globals."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !commanders"
    },
    {
      "script": "commander-help.cs",
      "action": "Commanders - Help",
      "purpose": "Displays commander help/routing information for chat.",
      "triggers": [
        "Twitch -> Chat Command -> !commanders"
      ],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Sends compact commander help text to chat."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for commander help"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
