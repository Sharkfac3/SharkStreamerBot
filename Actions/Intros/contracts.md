---
id: intros-contracts
type: contracts
description: Action contracts for Intros scripts.
owner: streamerbot-dev
secondaryOwners: [app-dev, brand-steward, ops]
parent: AGENTS.md
---

# Intros — Action Contracts

These contracts are maintained by `streamerbot-dev`. Load this file when validating or updating a script contract. Do not edit contracts without also updating the SHA256 stamp in the corresponding `.cs` file.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "first-chat-intro.cs",
      "action": "Intros - First Chat Intro",
      "purpose": "Play an approved custom intro sound for a viewer when their First Words trigger fires.",
      "triggers": [
        "Twitch -> Chat -> First Words"
      ],
      "globals": [
        "intro_sound_file_path"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [
        "http://127.0.0.1:8766"
      ],
      "requiredLiterals": [
        "user-intros",
        "enabled",
        "soundFile",
        "C:\\Users\\sharkfac3\\Workspace\\coding\\SharkStreamerBot\\Assets",
        "user-intros\\sound\\",
        "Intros - Play Custom Intro"
      ],
      "runtimeBehavior": [
        "Reads userId from First Words and no-ops when missing.",
        "GETs the user-intros record from info-service.",
        "Requires enabled record, soundFile, and existing local asset.",
        "Sets intro_sound_file_path, then runs Intros - Play Custom Intro synchronously."
      ],
      "failureBehavior": [
        "No-ops for missing userId, missing records, disabled intros, bad HTTP, malformed JSON, or missing files."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - First Chat Intro"
    },
    {
      "script": "redeem-capture.cs",
      "action": "Intros - Redeem Capture",
      "purpose": "Capture Custom Intro channel-point redemptions into the pending-intros info-service collection.",
      "triggers": [
        "Twitch -> Channel Reward -> Reward Redemption"
      ],
      "globals": [],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [
        "http://127.0.0.1:8766"
      ],
      "requiredLiterals": [
        "pending-intros",
        "redemptionId",
        "rewardName",
        "userName",
        "rawInput",
        "status",
        "pending"
      ],
      "runtimeBehavior": [
        "Reads redemption/user/reward/input fields from Reward Redemption, including userName as the Twitch login fallback for userLogin.",
        "No-ops when redemptionId is missing.",
        "GETs pending-intros by redemptionId and skips duplicates.",
        "POSTs a pending record keyed by redemptionId."
      ],
      "failureBehavior": [
        "No-ops for missing redemptionId, missing required record fields, duplicates, HTTP exceptions, or failed POST responses."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action: Intros - Redeem Capture"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
