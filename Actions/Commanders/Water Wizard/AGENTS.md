---
id: actions-commanders-water-wizard
type: domain-route
description: Water Wizard commander slot scripts, redeem behavior, !hail support command, and commander-only commands.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Water Wizard — Agent Guide

## Purpose

This folder owns Water Wizard-specific Streamer.bot C# scripts: the Water Wizard redeem, `!hail`, and Water Wizard-only `!hydrate`, `!orb`, and `!castrest` commands.
Use this guide for hydration/orb/cast-rest mechanics, Water Wizard cooldowns, and Water Wizard-specific support scoring.

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/Water Wizard/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or Water Wizard character identity.

## Ownership

Owner: `streamerbot-dev`. Chain to `brand-steward` for Water Wizard character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules.

## Required Reading

- [Actions/AGENTS.md](../../AGENTS.md) — shared Streamer.bot rules chain.
- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when Water Wizard characterization changes.

## Local Workflow

1. Preserve Water Wizard state variable names: `current_water_wizard`, `water_wizard_hail_count`, `water_wizard_hail_high_score`, `water_wizard_hail_high_score_user`, `water_wizard_hydrate_next_allowed_utc`, and `water_wizard_orb_next_allowed_utc`.
2. Keep `!hail` Water Wizard-specific: valid support increments `water_wizard_hail_count`; Water Wizard self-support stays blocked; outgoing redeem finalizes the hail high score.
3. Keep `!hydrate` Water Wizard-only with either numeric `1..10` or custom text up to `5` words, then start/refresh `water_wizard_hydrate_next_allowed_utc` for the 5-minute cooldown.
4. Keep `!orb` Water Wizard-only with optional text up to `30` words, `bowtome` mapped to the special branch, and `water_wizard_orb_next_allowed_utc` for the 1-minute cooldown.
5. Keep `!castrest` limited to an active rest/focus loop in `pre_rest`; set `rest_focus_loop_phase = "rest"` before arming `Rest Focus - Rest`, and fail-close the loop if timer arming fails.
6. Update the Action Contracts and Script Summary sections when Water Wizard trigger variables, command behavior, state variables, or operator wiring changes.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "water-wizard-redeem.cs",
      "action": "Commanders - Water Wizard - Redeem",
      "purpose": "Assigns Water Wizard, finalizes outgoing !hail score, checks high score, and fires the Mix It Up redeem command.",
      "triggers": [
        "Twitch -> Channel Reward -> Reward Redemption (Water Wizard reward)"
      ],
      "globals": [
        "current_water_wizard",
        "water_wizard_hail_count",
        "water_wizard_hydrate_next_allowed_utc",
        "water_wizard_orb_next_allowed_utc",
        "water_wizard_hail_high_score",
        "water_wizard_hail_high_score_user"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "d5452a4f-1bf3-4ce8-a6d8-dd7a74887752"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_hail_count"
      ],
      "runtimeBehavior": [
        "Reads current_water_wizard and finalizes outgoing !hail high-score state.",
        "Writes the new wizard to current_water_wizard.",
        "Resets !hail count and Water Wizard cooldown globals.",
        "Fires Mix It Up redeem command with new wizard identifiers."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for Water Wizard channel-point redeem"
    },
    {
      "script": "water-wizard-hail.cs",
      "action": "Commanders - Water Wizard - Hail",
      "purpose": "Handles public !hail support calls for the current Water Wizard. Increments the per-tenure hail count. Blocks self-support.",
      "triggers": [
        "Twitch -> Chat Command -> !hail"
      ],
      "globals": [
        "current_water_wizard",
        "water_wizard_hail_count"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_hail_count"
      ],
      "runtimeBehavior": [
        "Reads current_water_wizard defensively.",
        "Guides chat when no Water Wizard is active.",
        "Blocks Water Wizard from self-supporting.",
        "Increments water_wizard_hail_count and sends success chat."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !hail"
    },
    {
      "script": "wizard-hydrate.cs",
      "action": "Commanders - Water Wizard - Hydrate",
      "purpose": "Handles Water Wizard-only !hydrate with amount/message input, a 5-minute cooldown, and Mix It Up dispatch.",
      "triggers": [
        "Twitch -> Chat Command -> !hydrate"
      ],
      "globals": [
        "current_water_wizard",
        "water_wizard_hydrate_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "53244f6a-6882-4457-bc9f-b429ecd9ce9d"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_hydrate_next_allowed_utc",
        "amount",
        "message"
      ],
      "runtimeBehavior": [
        "Reads current_water_wizard defensively.",
        "Guides unauthorized callers toward !hail or redeem.",
        "Reports remaining cooldown to Water Wizard.",
        "Accepts amount 1-10 or custom text up to 5 words.",
        "Fires Mix It Up with hydrate message and type identifiers.",
        "Sets water_wizard_hydrate_next_allowed_utc to now plus 5 minutes."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !hydrate"
    },
    {
      "script": "water-wizard-orb.cs",
      "action": "Commanders - Water Wizard - Orb",
      "purpose": "Handles Water Wizard-only !orb with optional text, a 1-minute cooldown, and orbtype branching.",
      "triggers": [
        "Twitch -> Chat Command -> !orb"
      ],
      "globals": [
        "current_water_wizard",
        "water_wizard_orb_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "6b00a684-8fd4-404c-81b0-c279f241af73"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_orb_next_allowed_utc",
        "none",
        "message",
        "special",
        "bowtome"
      ],
      "runtimeBehavior": [
        "Reads current_water_wizard defensively.",
        "Guides unauthorized callers toward !hail or redeem.",
        "Reports remaining cooldown to Water Wizard.",
        "Validates optional orb text: 0-30 words.",
        "Sets orbtype to none, message, or special bowtome.",
        "Fires Mix It Up with orb message and type identifiers.",
        "Sets water_wizard_orb_next_allowed_utc to now plus 1 minute."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !orb"
    },
    {
      "script": "water-wizard-castrest.cs",
      "action": "Commanders - Water Wizard - Cast Rest",
      "purpose": "Lets Water Wizard set rest duration during pre_rest and arm the rest timer.",
      "triggers": [
        "Twitch -> Chat Command -> !castrest"
      ],
      "globals": [
        "current_water_wizard",
        "rest_focus_loop_active",
        "rest_focus_loop_phase"
      ],
      "timers": [
        "Rest Focus - Rest",
        "Rest Focus - Focus",
        "Rest Focus - Pre Rest",
        "Rest Focus - Pre Focus"
      ],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [
        "REPLACE_WITH_WIZARDS_REST_COMMAND_ID"
      ],
      "overlayTopics": [],
      "serviceUrls": [
        "http://localhost:8911/api/v2/commands/{commandId}"
      ],
      "requiredLiterals": [
        "current_water_wizard",
        "rest_focus_loop_active",
        "rest_focus_loop_phase",
        "pre_rest",
        "rest",
        "Rest Focus - Rest"
      ],
      "runtimeBehavior": [
        "Reads current_water_wizard defensively.",
        "Guides unauthorized callers toward !hail or default rest.",
        "Requires an active rest/focus loop in pre_rest phase.",
        "Parses whole minutes 1-999 from command input.",
        "Sets rest_focus_loop_phase to rest before arming the timer.",
        "Fires placeholder Mix It Up rest command with seconds.",
        "Disables non-target rest/focus timers.",
        "Re-arms Rest Focus - Rest with selected duration.",
        "Fail-closes the loop if timer arming fails."
      ],
      "failureBehavior": [
        "Mix It Up failure does not stop the loop.",
        "Timer-arm failure disables all loop timers and clears rest_focus_loop_active.",
        "Recovery requires fixing setup, then restarting the loop normally."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !castrest"
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
| `water-wizard-redeem.cs` | Assigns the new Water Wizard, finalizes outgoing `!hail` high score state, resets hail/cooldown globals, and fires the Water Wizard redeem Mix It Up command. |
| `water-wizard-hail.cs` | Handles public `!hail`, blocks Water Wizard self-support, increments `water_wizard_hail_count`, and guides chat when no Water Wizard is active. |
| `wizard-hydrate.cs` | Handles Water Wizard-only `!hydrate` with `1..10` or up-to-5-word input, Mix It Up hydrate identifiers, and a 5-minute cooldown. |
| `water-wizard-orb.cs` | Handles Water Wizard-only `!orb` with optional 0–30 word text, `bowtome` special branching, Mix It Up orb identifiers, and a 1-minute cooldown. |
| `water-wizard-castrest.cs` | Handles Water Wizard-only `!castrest` during `pre_rest`, dispatches rest seconds to Mix It Up, arms `Rest Focus - Rest`, and fail-closes on timer-arm failure. |
