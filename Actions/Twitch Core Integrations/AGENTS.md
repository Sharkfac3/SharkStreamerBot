---
id: actions-twitch-core-integrations
type: domain-route
description: Core Twitch stream lifecycle, follow, subscription, watch-streak, reset, and Mix It Up bridge guidance.
owner: streamerbot-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Core Integrations — Agent Guide

## Purpose

This folder owns core Twitch event and stream-state Streamer.bot C# scripts: stream-start reset, new follows, subscriptions, gift subscriptions, sub counter rollover, and watch streaks.
It keeps live stream state clean at startup and forwards Twitch event metadata to Mix It Up with stable payload shapes.
It is the local guide for the `Twitch Core Integrations` Streamer.bot action group.

## Owner

Owner: `streamerbot-dev`. Chain to `brand-steward` for follow/sub/watch-streak announcement wording. See `Actions/AGENTS.md` for full ownership rules.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Core Integrations/](./), including:

- [Actions/Twitch Core Integrations/stream-start.cs](stream-start.cs)
- [Actions/Twitch Core Integrations/follower-new.cs](follower-new.cs)
- [Actions/Twitch Core Integrations/subscription-new.cs](subscription-new.cs)
- [Actions/Twitch Core Integrations/subscription-renewed.cs](subscription-renewed.cs)
- [Actions/Twitch Core Integrations/subscription-prime-paid-upgrade.cs](subscription-prime-paid-upgrade.cs)
- [Actions/Twitch Core Integrations/subscription-gift-paid-upgrade.cs](subscription-gift-paid-upgrade.cs)
- [Actions/Twitch Core Integrations/subscription-pay-it-forward.cs](subscription-pay-it-forward.cs)
- [Actions/Twitch Core Integrations/subscription-gift.cs](subscription-gift.cs)
- [Actions/Twitch Core Integrations/subscription-counter-rollover.cs](subscription-counter-rollover.cs)
- [Actions/Twitch Core Integrations/watch-streak.cs](watch-streak.cs)
- Script Reference or operator documentation in this folder

## Required Reading

Read the following before editing scripts:

- [Actions/AGENTS.md](../AGENTS.md)
- [Actions/Squad/AGENTS.md](../Squad/AGENTS.md) when changing stream-start resets for Squad or mini-game lock state
- [Actions/LotAT/AGENTS.md](../LotAT/AGENTS.md) when changing stream-start resets for LotAT-related state

## Local Workflow

- `stream-start.cs` is the central stream-session reset point; it must run early in Streamer.bot startup.
- Any new global variable added by any feature must be added to `stream-start.cs` reset logic and documented in `Actions/SHARED-CONSTANTS.md`.
- Mix It Up payload compatibility: `Platform = "Twitch"`, empty `Arguments`, lowercase no-space keys in `SpecialIdentifiers`.
- The five dedicated subscription scripts each map to a distinct Twitch subscription trigger — preserve that mapping.
- `subscription-gift.cs` is the dual-trigger handler for Gift Subscription and Gift Bomb; preserve suppression of per-recipient events during gift bombs.

## Script Summary

| Script | Trigger category | Key globals reset/used | Purpose |
|---|---|---|---|
| `stream-start.cs` | Stream start / startup reset | Resets/uses `broker_connected`, `minigame_active`, `minigame_name`, `disco_party_active`, `disco_party_prev_scene`, `stream_mode`, `rarity_regular`, `rarity_smol`, `rarity_long`, `rarity_flight`, `rarity_party`, `last_roll`, `last_rarity`, `last_user`, `lotat_active`, `lotat_session_id`, `lotat_session_stage`, `lotat_session_story_id`, `lotat_session_current_node_id`, `lotat_session_chaos_total`, `lotat_session_roster_frozen`, `lotat_session_joined_roster_json`, `lotat_session_joined_count`, `lotat_node_active_window`, `lotat_node_window_resolved`, `lotat_node_allowed_commands_json`, `lotat_vote_map_json`, `lotat_vote_valid_count`, `lotat_node_commander_name`, `lotat_node_commander_target_user`, `lotat_node_commander_allowed_commands_json`, `lotat_node_dice_success_threshold`, `lotat_session_last_choice_id`, `lotat_session_last_end_state`, `lotat_announcement_sent`, `lotat_offering_steal_chance`, `lotat_steal_multiplier`, `duck_event_active`, `duck_quack_count`, `duck_caller`, `duck_unlocked`, `duck_target_quacks`, `duck_unique_quackers`, `duck_unique_quacker_count`, `empire_game_active`, `empire_join_active`, `empire_game_start_utc`, `empire_players_json`, `empire_cells_json`, `pedro_game_enabled`, `pedro_mention_count`, `pedro_unlocked`, `pedro_next_allowed_utc`, `pedro_secret_unlock_active`, `pedro_last_message_id`, `rest_focus_loop_active`, `rest_focus_loop_phase`, `xj_drivethrough_active`; preserves persisted `clone_unlocked` | Clears session state, timers, OBS source visibility, stream mode, and overlay broker registration for a fresh stream. |
| `follower-new.cs` | Twitch follow | None; uses trigger args only | Forwards new follower metadata to Mix It Up. |
| `subscription-new.cs` | Twitch subscription / new sub | None; uses trigger args only | Forwards first-time subscription metadata to Mix It Up. |
| `subscription-renewed.cs` | Twitch subscription / resubscription | None; uses trigger args only | Forwards resubscription metadata to Mix It Up. |
| `subscription-prime-paid-upgrade.cs` | Twitch subscription / Prime Paid Upgrade | None; uses trigger args only | Forwards Prime-to-paid upgrade metadata to Mix It Up. |
| `subscription-gift-paid-upgrade.cs` | Twitch subscription / Gift Paid Upgrade | None; uses trigger args only | Forwards gift-paid upgrade metadata to Mix It Up. |
| `subscription-pay-it-forward.cs` | Twitch subscription / Pay It Forward | None; uses trigger args only | Forwards pay-it-forward metadata to Mix It Up. |
| `subscription-gift.cs` | Twitch subscription / Gift Subscription + Gift Bomb | None; uses trigger args only | Routes solo gifts and gift bomb aggregates to Mix It Up, suppressing duplicate per-recipient bomb events. |
| `subscription-counter-rollover.cs` | Twitch subscription / Sub Counter Rollover | None; uses trigger args only | Forwards sub counter rollover metadata to Mix It Up. |
| `watch-streak.cs` | Twitch chat / Watch Streak | None; uses trigger args only | Forwards shared watch-streak metadata and viewer message state to Mix It Up. |

## Validation / Boundaries / Handoff

See `Actions/AGENTS.md` for universal validation, boundary, and handoff rules.

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
