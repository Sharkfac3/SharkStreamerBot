---
id: actions-commanders-captain-stretch
type: domain-route
description: Captain Stretch commander slot scripts, redeem behavior, !thank support command, and commander-only commands.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Captain Stretch — Agent Guide

## Purpose

Captain Stretch owns stretch/shrimp prompts, `!thank` support scoring, and the `!generalfocus` commander hook during rest/focus pre-focus windows.
Use the parent commander guide for the shared slot model; this file documents only Captain Stretch-specific behavior and state.

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/Captain Stretch/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or Captain Stretch character identity.

## Ownership

Owner: `streamerbot-dev`. Chain to `brand-steward` for Captain Stretch character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules.

## Required Reading

- [Actions/AGENTS.md](../../AGENTS.md) — shared Actions runtime, ownership, constants, helper, validation, and handoff rules.
- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when Captain Stretch characterization changes.

## Local Workflow

1. Preserve Captain Stretch-specific global names: `captain_stretch_stretch_next_allowed_utc`, `captain_stretch_shrimp_next_allowed_utc`, and the parent-documented slot/support/high-score keys.
2. Keep Captain Stretch command limits intact unless the operator asks for behavior changes: `!stretch` allows 0-10 words and max 40 characters; `!shrimp` allows 0-30 words.
3. Keep `!stretch` on a 5-minute cooldown and `!shrimp` on a 1-minute cooldown, stored as UTC Unix timestamps.
4. Keep `!generalfocus` limited to the active rest/focus loop `pre_focus` phase and fail closed if timer arming fails.
5. Update this file when Captain Stretch trigger variables, command behavior, state variables, or operator wiring changes.

## Captain Stretch State

State already listed in the parent commander `SHARED-STATE` remains authoritative for the active slot, `!thank` counter, and `!thank` high-score keys.

Captain Stretch-only cooldown globals:

| Global | Purpose |
|---|---|
| `captain_stretch_stretch_next_allowed_utc` | UTC Unix timestamp for the next allowed `!stretch` use. |
| `captain_stretch_shrimp_next_allowed_utc` | UTC Unix timestamp for the next allowed `!shrimp` use. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "captain-stretch-redeem.cs",
      "action": "Commanders - Captain Stretch - Redeem",
      "purpose": "Assigns Captain Stretch, finalizes outgoing !thank score, checks high score, and fires the Mix It Up redeem command.",
      "triggers": [
        "Twitch -> Channel Reward -> Reward Redemption (Captain Stretch reward)"
      ],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_thank_count",
        "captain_stretch_stretch_next_allowed_utc",
        "captain_stretch_shrimp_next_allowed_utc",
        "captain_stretch_thank_high_score",
        "captain_stretch_thank_high_score_user"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "1facdbe6-f292-4d1b-9b66-ad71ae6de310"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_captain_stretch",
        "captain_stretch_thank_count"
      ],
      "runtimeBehavior": [
        "Reads current_captain_stretch and finalizes outgoing !thank high-score state.",
        "Writes the new captain to current_captain_stretch.",
        "Resets !thank count and Captain Stretch cooldown globals.",
        "Fires Mix It Up redeem command with new captain identifiers."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for Captain Stretch channel-point redeem"
    },
    {
      "script": "captain-stretch-thank.cs",
      "action": "Commanders - Captain Stretch - Thank",
      "purpose": "Handles public !thank support calls for the current Captain Stretch. Increments the per-tenure thank count. Blocks self-support.",
      "triggers": [
        "Twitch -> Chat Command -> !thank"
      ],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_thank_count"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [
        "current_captain_stretch",
        "captain_stretch_thank_count"
      ],
      "runtimeBehavior": [
        "Reads current_captain_stretch defensively.",
        "Guides chat when no Captain Stretch is active.",
        "Blocks Captain Stretch from self-supporting.",
        "Increments captain_stretch_thank_count and sends success chat."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !thank"
    },
    {
      "script": "captain-stretch-stretch.cs",
      "action": "Commanders - Captain Stretch - Stretch",
      "purpose": "Handles Captain Stretch-only !stretch with input limits, a 5-minute cooldown, and Mix It Up dispatch.",
      "triggers": [
        "Twitch -> Chat Command -> !stretch"
      ],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_stretch_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "60b43da9-accb-4dbe-968a-d57846a7dc4c"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_captain_stretch",
        "captain_stretch_stretch_next_allowed_utc"
      ],
      "runtimeBehavior": [
        "Reads current_captain_stretch defensively.",
        "Guides unauthorized callers toward !thank or redeem.",
        "Reports remaining cooldown to Captain Stretch.",
        "Validates optional phrase: 0-10 words, max 40 characters.",
        "Fires Mix It Up with the validated stretch phrase.",
        "Sets captain_stretch_stretch_next_allowed_utc to now plus 5 minutes."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !stretch"
    },
    {
      "script": "captain-stretch-shrimp.cs",
      "action": "Commanders - Captain Stretch - Shrimp",
      "purpose": "Handles Captain Stretch-only !shrimp with input limits, a 1-minute cooldown, and Mix It Up dispatch.",
      "triggers": [
        "Twitch -> Chat Command -> !shrimp"
      ],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_shrimp_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "af5567d1-ac94-49bf-ad7b-0b7e034cb05d"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_captain_stretch",
        "captain_stretch_shrimp_next_allowed_utc"
      ],
      "runtimeBehavior": [
        "Reads current_captain_stretch defensively.",
        "Guides unauthorized callers toward !thank or redeem.",
        "Reports remaining cooldown to Captain Stretch.",
        "Validates optional phrase: 0-30 words.",
        "Fires Mix It Up with the validated shrimp phrase.",
        "Sets captain_stretch_shrimp_next_allowed_utc to now plus 1 minute."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !shrimp"
    },
    {
      "script": "captain-stretch-generalfocus.cs",
      "action": "Commanders - Captain Stretch - General Focus",
      "purpose": "Lets Captain Stretch set focus duration during pre_focus and arm the focus timer.",
      "triggers": [
        "Twitch -> Chat Command -> !generalfocus"
      ],
      "globals": [
        "current_captain_stretch",
        "rest_focus_loop_active",
        "rest_focus_loop_phase"
      ],
      "timers": [
        "Rest Focus - Focus",
        "Rest Focus - Rest",
        "Rest Focus - Pre Focus",
        "Rest Focus - Pre Rest"
      ],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_captain_stretch",
        "rest_focus_loop_active",
        "rest_focus_loop_phase",
        "pre_focus",
        "focus",
        "Rest Focus - Focus"
      ],
      "runtimeBehavior": [
        "Reads current_captain_stretch defensively.",
        "Guides unauthorized callers toward !thank or default focus.",
        "Requires an active rest/focus loop in pre_focus phase.",
        "Parses whole minutes 1-999 from command input.",
        "Sets rest_focus_loop_phase to focus before arming the timer.",
        "Fires placeholder Mix It Up focus command with seconds.",
        "Disables non-target rest/focus timers.",
        "Re-arms Rest Focus - Focus with selected duration.",
        "Fail-closes the loop if timer arming fails."
      ],
      "failureBehavior": [
        "Mix It Up failure does not stop the loop.",
        "Timer-arm failure disables all loop timers and clears rest_focus_loop_active.",
        "Recovery requires fixing setup, then restarting the loop normally."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !generalfocus"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

## Boundaries / Out of Scope

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

## Handoff Notes

See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules.

---

## Script Summary

| Script | Summary |
|---|---|
| `captain-stretch-redeem.cs` | Assigns the new Captain Stretch, finalizes outgoing `!thank` high-score state, resets `!thank` and command cooldown globals, then fires Mix It Up redeem command `1facdbe6-f292-4d1b-9b66-ad71ae6de310`. |
| `captain-stretch-thank.cs` | Handles public `!thank`, blocks self-support, increments `captain_stretch_thank_count`, and guides chat when no Captain Stretch is active. |
| `captain-stretch-stretch.cs` | Allows only the active Captain Stretch to run `!stretch` with optional 0-10 word / 40-character phrase, 5-minute cooldown, and Mix It Up command `60b43da9-accb-4dbe-968a-d57846a7dc4c`. |
| `captain-stretch-shrimp.cs` | Allows only the active Captain Stretch to run `!shrimp` with optional 0-30 word phrase, 1-minute cooldown, and Mix It Up command `af5567d1-ac94-49bf-ad7b-0b7e034cb05d`. |
| `captain-stretch-generalfocus.cs` | Allows only the active Captain Stretch to set a 1-999 minute focus duration during `pre_focus`, arms `Rest Focus - Focus`, calls placeholder Mix It Up command `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID`, and fail-closes the loop on timer-arm failure. |
