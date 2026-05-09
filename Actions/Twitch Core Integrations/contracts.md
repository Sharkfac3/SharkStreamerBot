---
id: twitch-core-contracts
type: contracts
description: Action contracts for Twitch Core Integration scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# Twitch Core Integrations — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "follower-new.cs",
      "action": "Follower New",
      "purpose": "Contracts expected runtime behavior for follower-new.cs.",
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
        "Runs the documented follower-new.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for follower-new.cs"
    },
    {
      "script": "stream-start.cs",
      "action": "Stream Start",
      "purpose": "Contracts expected runtime behavior for stream-start.cs.",
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
        "Runs the documented stream-start.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for stream-start.cs"
    },
    {
      "script": "subscription-counter-rollover.cs",
      "action": "Subscription Counter Rollover",
      "purpose": "Contracts expected runtime behavior for subscription-counter-rollover.cs.",
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
        "Runs the documented subscription-counter-rollover.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-counter-rollover.cs"
    },
    {
      "script": "subscription-gift-paid-upgrade.cs",
      "action": "Subscription Gift Paid Upgrade",
      "purpose": "Contracts expected runtime behavior for subscription-gift-paid-upgrade.cs.",
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
        "Runs the documented subscription-gift-paid-upgrade.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-gift-paid-upgrade.cs"
    },
    {
      "script": "subscription-gift.cs",
      "action": "Subscription Gift",
      "purpose": "Contracts expected runtime behavior for subscription-gift.cs.",
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
        "Runs the documented subscription-gift.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-gift.cs"
    },
    {
      "script": "subscription-new.cs",
      "action": "Subscription New",
      "purpose": "Contracts expected runtime behavior for subscription-new.cs.",
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
        "Runs the documented subscription-new.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-new.cs"
    },
    {
      "script": "subscription-pay-it-forward.cs",
      "action": "Subscription Pay It Forward",
      "purpose": "Contracts expected runtime behavior for subscription-pay-it-forward.cs.",
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
        "Runs the documented subscription-pay-it-forward.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-pay-it-forward.cs"
    },
    {
      "script": "subscription-prime-paid-upgrade.cs",
      "action": "Subscription Prime Paid Upgrade",
      "purpose": "Contracts expected runtime behavior for subscription-prime-paid-upgrade.cs.",
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
        "Runs the documented subscription-prime-paid-upgrade.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-prime-paid-upgrade.cs"
    },
    {
      "script": "subscription-renewed.cs",
      "action": "Subscription Renewed",
      "purpose": "Contracts expected runtime behavior for subscription-renewed.cs.",
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
        "Runs the documented subscription-renewed.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for subscription-renewed.cs"
    },
    {
      "script": "watch-streak.cs",
      "action": "Watch Streak",
      "purpose": "Contracts expected runtime behavior for watch-streak.cs.",
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
        "Runs the documented watch-streak.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for watch-streak.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
