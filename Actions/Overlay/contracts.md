---
id: overlay-contracts
type: contracts
description: Action contracts for Overlay WebSocket broker scripts.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: AGENTS.md
---

# Overlay — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "broker-connect.cs",
      "action": "Broker Connect",
      "purpose": "Contracts expected runtime behavior for broker-connect.cs.",
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
        "Runs the documented broker-connect.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-connect.cs"
    },
    {
      "script": "broker-disconnect.cs",
      "action": "Broker Disconnect",
      "purpose": "Contracts expected runtime behavior for broker-disconnect.cs.",
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
        "Runs the documented broker-disconnect.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-disconnect.cs"
    },
    {
      "script": "broker-publish.cs",
      "action": "Broker Publish",
      "purpose": "Contracts expected runtime behavior for broker-publish.cs.",
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
        "Runs the documented broker-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-publish.cs"
    },
    {
      "script": "test-overlay.cs",
      "action": "Test Overlay",
      "purpose": "Contracts expected runtime behavior for test-overlay.cs.",
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
        "Runs the documented test-overlay.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for test-overlay.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
