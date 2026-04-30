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

This folder owns the Streamer.bot C# scripts for the Water Wizard commander slot: role assignment via channel-point redeem, `!hail` public support command, and Water Wizard-only commands (`!hydrate`, `!orb`, `!castrest`).

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/Water Wizard/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or Water Wizard character identity.

## Primary Owner

`streamerbot-dev` owns C# runtime behavior, paste readiness, global variable use, and command wiring expectations.

## Secondary Owners / Chain To

- `brand-steward` — chain for public chat text, Water Wizard character voice, reward descriptions, or naming changes.
- `ops` — chain for validation, paste/sync workflow, or agent-tree maintenance.

## Required Reading

- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md)
- [Actions/Helpers/AGENTS.md](../../Helpers/AGENTS.md)
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when Water Wizard characterization changes.

## Local Workflow

1. Read the parent [Actions/Commanders/AGENTS.md](../AGENTS.md) for the shared commander slot model and support command rules.
2. Preserve the Water Wizard slot model — one active slot, backward-compatible redeem behavior.
3. Do not rename `current_water_wizard`, `water_wizard_hail_count`, or high-score keys without explicit operator request.
4. Read runtime state via `CPH.TryGetArg` or Streamer.bot globals defensively; protect against missing or malformed inputs.
5. Keep scripts self-contained and paste-ready. Do not assume shared runtime files can be imported by Streamer.bot actions.
6. Update the Action Contracts and Script Reference sections in this file when trigger variables, command behavior, state variables, or operator wiring changes.
7. If a new global variable, OBS source, timer, or command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md) or flag it in the handoff.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "water-wizard-redeem.cs",
      "action": "Commanders - Water Wizard - Redeem",
      "purpose": "Assigns the current Water Wizard slot, finalizes the outgoing wizard's !hail score, checks for a new all-time high score, and fires the Mix It Up redeem command.",
      "triggers": ["Twitch -> Channel Reward -> Reward Redemption (Water Wizard reward)"],
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
      "mixItUpCommandIds": ["d5452a4f-1bf3-4ce8-a6d8-dd7a74887752"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_water_wizard", "water_wizard_hail_count"],
      "runtimeBehavior": [
        "Read outgoing wizard from current_water_wizard.",
        "If outgoing wizard's hail count exceeds persisted high score, announce new record in chat and update high score vars.",
        "Write new wizard username to current_water_wizard.",
        "Reset water_wizard_hail_count to 0.",
        "Reset water_wizard_hydrate_next_allowed_utc and water_wizard_orb_next_allowed_utc so new wizard starts with no cooldown debt.",
        "Fire Mix It Up command with new wizard username as Arguments, SpecialIdentifiers.user, and SpecialIdentifiers.commander."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Role assignment still completes.",
        "Return true after slot update."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for Water Wizard channel-point redeem"
    },
    {
      "script": "water-wizard-hail.cs",
      "action": "Commanders - Water Wizard - Hail",
      "purpose": "Handles public !hail support calls for the current Water Wizard. Increments the per-tenure hail count. Blocks self-support.",
      "triggers": ["Twitch -> Chat Command -> !hail"],
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
      "requiredLiterals": ["current_water_wizard", "water_wizard_hail_count"],
      "runtimeBehavior": [
        "Read current_water_wizard defensively.",
        "If no active Water Wizard, send guidance message and return.",
        "If caller is current Water Wizard, block self-support and return.",
        "Increment water_wizard_hail_count by 1.",
        "Send success message in chat."
      ],
      "failureBehavior": [
        "Missing or blank current_water_wizard treated as no active wizard.",
        "Return true after guidance or success message."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !hail"
    },
    {
      "script": "wizard-hydrate.cs",
      "action": "Commanders - Water Wizard - Hydrate",
      "purpose": "Handles Water Wizard-only !hydrate command. Accepts a number 1-10 or short custom text (0-5 words), enforces 5-minute cooldown, fires Mix It Up command with hydratetype branching.",
      "triggers": ["Twitch -> Chat Command -> !hydrate"],
      "globals": [
        "current_water_wizard",
        "water_wizard_hydrate_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["53244f6a-6882-4457-bc9f-b429ecd9ce9d"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_hydrate_next_allowed_utc",
        "amount",
        "message"
      ],
      "runtimeBehavior": [
        "Read current_water_wizard defensively.",
        "If caller is not current Water Wizard: if slot active, send !hail instruction; else encourage redeem.",
        "If caller is Water Wizard and on cooldown, send cooldown remaining message.",
        "Parse input from input0, fallback rawInput/message. Accept number 1-10 (hydratetype = 'amount') or short custom text up to 5 words (hydratetype = 'message').",
        "Fire Mix It Up command with parsed hydrate payload as Arguments, SpecialIdentifiers.hydratemessage, and SpecialIdentifiers.hydratetype.",
        "Refresh water_wizard_hydrate_next_allowed_utc to now + 5 minutes."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !hydrate"
    },
    {
      "script": "water-wizard-orb.cs",
      "action": "Commanders - Water Wizard - Orb",
      "purpose": "Handles Water Wizard-only !orb command. Accepts optional text (0-30 words), enforces 1-minute cooldown, fires Mix It Up command with orbtype branching (none/message/special).",
      "triggers": ["Twitch -> Chat Command -> !orb"],
      "globals": [
        "current_water_wizard",
        "water_wizard_orb_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["6b00a684-8fd4-404c-81b0-c279f241af73"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": [
        "current_water_wizard",
        "water_wizard_orb_next_allowed_utc",
        "none",
        "message",
        "special",
        "bowtome"
      ],
      "runtimeBehavior": [
        "Read current_water_wizard defensively.",
        "If caller is not current Water Wizard: if slot active, send !hail instruction; else encourage redeem.",
        "If caller is Water Wizard and on cooldown, send cooldown remaining message.",
        "Validate input: 0 to 30 words (reads rawInput, fallback message). !orb by itself is valid.",
        "Set orbtype = 'none' if no input, 'special' if input is exactly 'bowtome' (case-insensitive), otherwise 'message'.",
        "Fire Mix It Up command with validated orb text as Arguments, SpecialIdentifiers.orbmessage, and SpecialIdentifiers.orbtype.",
        "Refresh water_wizard_orb_next_allowed_utc to now + 1 minute."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !orb"
    },
    {
      "script": "water-wizard-castrest.cs",
      "action": "Commanders - Water Wizard - Cast Rest",
      "purpose": "Handles Water Wizard-only !castrest X command during the rest/focus loop's pre_rest window. Sets the operator-selected rest duration and arms the rest timer.",
      "triggers": ["Twitch -> Chat Command -> !castrest"],
      "globals": [
        "current_water_wizard",
        "rest_focus_loop_active",
        "rest_focus_loop_phase"
      ],
      "timers": ["Rest Focus - Rest", "Rest Focus - Focus", "Rest Focus - Pre Rest", "Rest Focus - Pre Focus"],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["REPLACE_WITH_WIZARDS_REST_COMMAND_ID"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": [
        "current_water_wizard",
        "rest_focus_loop_active",
        "rest_focus_loop_phase",
        "pre_rest",
        "rest",
        "Rest Focus - Rest"
      ],
      "runtimeBehavior": [
        "Read current_water_wizard defensively.",
        "If caller is not current Water Wizard: if slot active, send !hail instruction; else explain default rest duration will be used.",
        "If loop is not active, send short message that crew is not in a rest/focus loop.",
        "If loop is not in pre_rest phase, send short message that the rest window already moved on.",
        "Parse X as whole minutes 1-999 from input0, fallback rawInput/message.",
        "Set rest_focus_loop_phase = 'rest' before arming timer.",
        "Fire Mix It Up placeholder command with duration in seconds as Arguments and SpecialIdentifiers.time.",
        "Disable all non-target rest/focus loop timers.",
        "Disable and re-arm timer 'Rest Focus - Rest' with X minutes converted to seconds.",
        "If timer arming fails, disable all four loop timers and set rest_focus_loop_active = false."
      ],
      "failureBehavior": [
        "Mix It Up failure does not stop the loop.",
        "Timer-arm failure disables all four rest/focus timers and clears rest_focus_loop_active.",
        "Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !castrest"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no external imports beyond what Streamer.bot supports, no repo-only helper file dependencies.
- Verify all global names, timers, and Mix It Up command IDs against [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

- For action contract changes, run:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --changed
```

## Boundaries / Out of Scope

- Do not rename `current_water_wizard`, `water_wizard_hail_count`, or high-score keys without explicit operator request.
- Do not change Water Wizard character identity or public copy without `brand-steward` review.
- Do not migrate Captain Stretch, The Director, Squad, Voice Commands, LotAT, or overlay behavior into this guide.

## Handoff Notes

After changes, follow:

- [change-summary](../../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files. Operator must manually paste changed script contents into matching Streamer.bot actions and verify trigger wiring.

Brand handoff triggers: public chat copy, reward wording, command help text, Water Wizard identity, or character/lore changes.

---

## Script Reference

### Script: `water-wizard-redeem.cs`

#### Purpose
Assigns the current Water Wizard commander slot occupant, finalizes the outgoing wizard's `!hail` score, and checks for a new all-time high score.

#### Expected Trigger / Input
- Redeem/action trigger for Water Wizard role assignment.
- Reads `user`.

#### Required Runtime Variables
- Reads/writes `current_water_wizard`.
- Reads/writes `water_wizard_hail_count`.
- Reads/writes `water_wizard_hydrate_next_allowed_utc`.
- Reads/writes `water_wizard_orb_next_allowed_utc`.
- Reads/writes (persisted) `water_wizard_hail_high_score`.
- Reads/writes (persisted) `water_wizard_hail_high_score_user`.

#### Key Outputs / Side Effects
- Updates active Water Wizard commander assignment.
- Resets `water_wizard_hail_count` to `0` for the new wizard tenure.
- Resets all Water Wizard command cooldown vars so the new wizard starts with no cooldown debt.
- If outgoing wizard beat the high score, announces the new record in chat.
- Triggers Mix It Up command `Commander - Water Wizard - Redeem` after the new wizard is stored.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `d5452a4f-1bf3-4ce8-a6d8-dd7a74887752`
- Payload `Arguments`: new Water Wizard username
- Payload `SpecialIdentifiers.user`: new Water Wizard username
- Payload `SpecialIdentifiers.commander`: new Water Wizard username

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Announces new high score when record is beaten.

#### Operator Notes
- Keep variable key `current_water_wizard` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

### Script: `water-wizard-hail.cs`

#### Purpose
Handles public `!hail` support calls for the current Water Wizard.

#### Expected Trigger / Input
- Chat command/action trigger for `!hail`.
- Reads `user`.

#### Required Runtime Variables
- Reads `current_water_wizard`.
- Reads/writes `water_wizard_hail_count`.

#### Key Outputs / Side Effects
- Increments `water_wizard_hail_count` by 1 per valid `!hail`.
- Blocks self-support (current Water Wizard cannot `!hail` themselves).
- If no active Water Wizard exists, responds with guidance.

#### Mix It Up Actions
- None currently.

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends success/failure guidance messages in chat.

#### Operator Notes
- Future Mix It Up hook can be added to the success path.

---

### Script: `wizard-hydrate.cs`

#### Purpose
Handles Water Wizard-only `!hydrate` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!hydrate`.
- Accepts either:
  - a number from `1` to `10`, or
  - a short custom message up to `5` words.
- Reads `input0` first, then falls back to `rawInput` / `message`.

#### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads/writes `water_wizard_hydrate_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current Water Wizard and hydrate input is valid and off cooldown:
  - Triggers Mix It Up command and forwards the parsed hydrate payload as `Arguments`.
  - Also sends `SpecialIdentifiers.hydratemessage` and `SpecialIdentifiers.hydratetype` so Mix It Up can branch intentionally.
  - Uses `hydratetype = "amount"` for numeric `1..10` input.
  - Uses `hydratetype = "message"` for short custom text.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail` for encouragement.
  - If no Water Wizard is active, encourages caller to redeem and become the Water Wizard.
- If caller **is** current Water Wizard but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `53244f6a-6882-4457-bc9f-b429ecd9ce9d`
- Payload `Arguments`: the validated hydrate payload text (either `1..10` or the short custom message)
- Payload `SpecialIdentifiers.hydratemessage`: same validated hydrate payload text
- Payload `SpecialIdentifiers.hydratetype`: `amount` or `message`

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Invalid usage guidance explains both accepted input shapes: numeric `1..10` or short custom text up to `5` words.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to the `!hydrate` command trigger action.
- Update the receiving Mix It Up command to use `$hydratemessage` and `$hydratetype` instead of assuming hydrate input is always numeric.
- Cooldown is role-specific and tracked globally by key `water_wizard_hydrate_next_allowed_utc`.

---

### Script: `water-wizard-castrest.cs`

#### Purpose
Handles Water Wizard-only `!castrest X` command usage during the rest/focus loop's pre-rest window.

#### Expected Trigger / Input
- Chat command/action trigger for `!castrest`.
- Expects `X` to be a whole number of minutes from `1` to `999` (reads `input0` first, then falls back to `rawInput` / `message`).

#### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads `rest_focus_loop_active`.
- Reads/writes `rest_focus_loop_phase`.

#### Key Outputs / Side Effects
- If caller **is** current Water Wizard and the loop is in `pre_rest`:
  - Sets `rest_focus_loop_phase = "rest"` before arming the next timer so overlapping triggers see the intended target state.
  - Triggers Mix It Up placeholder command `Wizards Rest` with `Arguments = seconds` and `SpecialIdentifiers.time = seconds`.
  - Defensively disables every non-target rest/focus loop timer, then disables and re-arms timer `Rest Focus - Rest` using `X` minutes converted to seconds.
  - If timer arming fails, disables all four loop timers and marks `rest_focus_loop_active = false` so the operator can restart cleanly.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail`.
  - If no Water Wizard is active, explains that the default rest duration will be used instead.
- If caller **is** current Water Wizard but the loop is not active:
  - Sends a short chat message that the crew is not in a rest/focus loop right now.
- If caller **is** current Water Wizard but the loop is not in `pre_rest`:
  - Sends a short chat message explaining that the rest window already moved on.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_WIZARDS_REST_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: rest duration in seconds
- Payload `SpecialIdentifiers.time`: rest duration in seconds

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends short guidance messages for unauthorized use, invalid usage, inactive loop state, and wrong loop phase.
- Logs warning/error if Mix It Up call fails.
- Logs timer arming attempts, timer-arm failures, and fail-closed loop recovery actions.

#### Operator Notes
- Replace `REPLACE_WITH_WIZARDS_REST_COMMAND_ID` before production use.
- Wire this script to the `!castrest` command trigger action.
- This script depends on `CPH.SetTimerInterval(string, int)` to apply the operator-selected rest duration. Verify that method in your Streamer.bot build before production use.
- Mix It Up failure does **not** stop the loop. Timer-arm failure does.
- If timer arming fails, the script disables all four rest/focus timers and clears `rest_focus_loop_active`. Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action.

---

### Script: `water-wizard-orb.cs`

#### Purpose
Handles Water Wizard-only `!orb` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!orb`.
- Accepts optional text after the command (`0` to `30` words; reads `rawInput`, then fallback `message`).

#### Required Runtime Variables
- Reads `current_water_wizard` (active Water Wizard username).
- Reads/writes `water_wizard_orb_next_allowed_utc` (Unix timestamp, UTC, used for 1-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current Water Wizard and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards optional orb text (`0` to `30` words) as payload `Arguments`.
  - Also sends `SpecialIdentifiers.orbmessage` and `SpecialIdentifiers.orbtype` so Mix It Up can branch intentionally.
  - Uses `orbtype = "none"` when no orb text was provided.
  - Uses `orbtype = "message"` for normal custom text.
  - Uses `orbtype = "special"` when the user message is exactly `bowtome` (case-insensitive).
  - Starts/refreshes 1-minute cooldown.
- If caller **is not** current Water Wizard:
  - If a Water Wizard is active, sends Twitch chat instruction to type `!hail` for encouragement.
  - If no Water Wizard is active, encourages caller to redeem and become the Water Wizard.
- If caller **is** current Water Wizard but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `6b00a684-8fd4-404c-81b0-c279f241af73`
- Payload `Arguments`: validated orb text (optional, max 30 words)
- Payload `SpecialIdentifiers.orbmessage`: same validated orb text
- Payload `SpecialIdentifiers.orbtype`: `none`, `message`, or `special`

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to the `!orb` command trigger action.
- Update the receiving Mix It Up command to use `$orbmessage` and `$orbtype` if you want a dedicated branch for the `bowtome` special case.
