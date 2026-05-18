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

`first-chat-intro.cs` is the authoritative gatekeeper for first-chat custom intro playback. It owns: deciding whether any intro should run at all, resolving filename-based local assets into full paths, and dispatching the Mix It Up `Custom Intro` command directly via the helper-pattern API call when at least one local asset exists.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "first-chat-intro.cs",
      "action": "Intros - First Chat Intro",
      "purpose": "Resolve approved custom intro assets for a viewer and dispatch the Mix It Up Custom Intro command directly when their First Words trigger fires.",
      "triggers": [
        "Twitch -> Chat -> First Words"
      ],
      "globals": [],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "REPLACE_WITH_CUSTOM_INTRO_COMMAND_ID"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://127.0.0.1:8766",
        "http://localhost:8911"
      ],
      "requiredLiterals": [
        "user-intros",
        "enabled",
        "soundFile",
        "gifFile",
        "C:\\Users\\sharkfac3\\Workspace\\coding\\SharkStreamerBot\\Assets",
        "user-intros\\sound\\",
        "user-intros\\gif\\",
        "http://localhost:8911",
        "REPLACE_WITH_CUSTOM_INTRO_COMMAND_ID",
        "intro_sound_file_path",
        "intro_gif_file_path"
      ],
      "runtimeBehavior": [
        "Reads userId from First Words and no-ops when missing.",
        "GETs the user-intros record from info-service.",
        "Requires an enabled record and at least one usable intro asset filename.",
        "Normalizes soundFile and gifFile to filename-only values, then resolves them under the local Assets tree when present.",
        "Supports sound-only, gif-only, and sound+gif dispatches.",
        "Calls the Mix It Up command API directly with SpecialIdentifiers.intro_sound_file_path and SpecialIdentifiers.intro_gif_file_path when at least one asset exists locally."
      ],
      "failureBehavior": [
        "No-ops for missing userId, missing records, disabled intros, bad HTTP, malformed JSON, normalized-empty filenames, command ID not configured, Mix It Up call failure, or cases where no configured asset resolves to a local file."
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
