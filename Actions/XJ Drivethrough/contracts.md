---
id: xj-drivethrough-contracts
type: contracts
description: Action contracts for XJ Drivethrough scripts.
owner: streamerbot-dev
parent: AGENTS.md
---

# XJ Drivethrough — Action Contracts

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "xj-drivethrough-main.cs",
      "action": "XJ Drivethrough / Main",
      "purpose": "Handles !xj chance-gated drivethroughs plus commander XJ pieces and 10-second triforce combos.",
      "triggers": [
        "Twitch -> Chat Command -> !xj",
        "Optional operator-wired manual trigger for testing"
      ],
      "globals": [
        "xj_drivethrough_active",
        "broker_connected",
        "current_water_wizard",
        "current_captain_stretch",
        "current_the_director",
        "xj_commander_triforce_active",
        "xj_commander_triforce_seen_json",
        "xj_commander_triforce_deadline_utc",
        "xj_commander_triforce_count",
        "xj_commander_triforce_high_score"
      ],
      "timers": [
        "XJ - Commander Triforce Window"
      ],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [
        "overlay.spawn",
        "overlay.move",
        "overlay.audio.play",
        "overlay.audio.stop",
        "overlay.remove"
      ],
      "serviceUrls": [],
      "requiredLiterals": [
        "streamerbot",
        "xj-drivethrough",
        "images/xj-drivethrough.png",
        "images/xj-left.png",
        "images/xj-middle.png",
        "images/xj-right.png",
        "xj-left",
        "xj-center",
        "xj-right",
        "xj-rev-limiter",
        "audio/xj-rev-limiter.mp3",
        "XJ_CHANCE_MIN",
        "XJ_CHANCE_MAX_EXCLUSIVE",
        "XJ_TRIGGER_THRESHOLD",
        "WAIT_SPAWN_SETTLE_MS",
        "WAIT_POST_DRIVE_MS",
        "XJ_COMMANDER_DISPLAY_MS",
        "XJ_COMMANDER_TRIFORCE_WINDOW_MS"
      ],
      "runtimeBehavior": [
        "Matches caller against active commander slot globals case-insensitively.",
        "Commander callers bypass non-commander chance gate.",
        "Non-commanders roll 1-100 before claiming xj_drivethrough_active.",
        "Non-commander rolls 75 or lower send no broker messages.",
        "Passed non-commander rolls claim the re-entry guard.",
        "Duplicate non-commander requests drop while guard is active.",
        "Spawns non-commander XJ off-screen left without enter animation.",
        "Waits briefly before move so asset loading settles.",
        "Moves XJ across 1920x1080 over 10000ms.",
        "Starts rev-limiter audio immediately after move publish.",
        "Stops audio and removes XJ after drive plus buffer.",
        "Releases xj_drivethrough_active in finally after claiming guard.",
        "Water Wizard spawns left XJ piece for 10000ms.",
        "Captain Stretch spawns center XJ piece for 10000ms.",
        "The Director spawns right XJ piece for 10000ms.",
        "Multi-role users spawn every matching commander piece.",
        "Commander pieces use stable per-piece asset IDs.",
        "Commander cleanup relies on overlay.spawn lifetime.",
        "Only commander calls affect triforce state.",
        "Tracks current-stream count separately from persisted high score.",
        "First commander role opens the triforce timer window.",
        "Active triforce windows count each commander role once.",
        "Completing all three roles increments count, updates high score, chats, plays audio, and clears state.",
        "Expired triforce windows clear state without changing counts or playing audio."
      ],
      "failureBehavior": [
        "Non-commander broker publish failure logs and relies on finally guard release.",
        "Commander spawn failure logs while keeping triforce state consistent.",
        "Disconnected WebSocket attempts reconnect and clears broker_connected if reconnect fails."
      ],
      "pasteTarget": "Streamer.bot action that runs the XJ Drivethrough main Execute C# Code sub-action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->
