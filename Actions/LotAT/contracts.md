---
id: lotat-contracts
type: contracts
description: Action contracts for all LotAT runtime scripts.
owner: lotat-tech
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# LotAT — Action Contracts

These contracts are maintained by `lotat-tech`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

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
