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

This folder owns the Streamer.bot C# scripts for the Captain Stretch commander slot: role assignment via channel-point redeem, `!thank` public support command, and Captain Stretch-only commands (`!stretch`, `!shrimp`, `!generalfocus`).

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/Captain Stretch/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or Captain Stretch character identity.

## Primary Owner

`streamerbot-dev` owns C# runtime behavior, paste readiness, global variable use, and command wiring expectations.

## Secondary Owners / Chain To

- `brand-steward` — chain for public chat text, Captain Stretch character voice, reward descriptions, or naming changes.
- `ops` — chain for validation, paste/sync workflow, or agent-tree maintenance.

## Required Reading

- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md)
- [Actions/Helpers/AGENTS.md](../../Helpers/AGENTS.md)
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when Captain Stretch characterization changes.

## Local Workflow

1. Read the parent [Actions/Commanders/AGENTS.md](../AGENTS.md) for the shared commander slot model and support command rules.
2. Preserve the Captain Stretch slot model — one active slot, backward-compatible redeem behavior.
3. Do not rename `current_captain_stretch`, `captain_stretch_thank_count`, or high-score keys without explicit operator request.
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
      "script": "captain-stretch-redeem.cs",
      "action": "Commanders - Captain Stretch - Redeem",
      "purpose": "Assigns the current Captain Stretch slot, finalizes the outgoing captain's !thank score, checks for a new all-time high score, and fires the Mix It Up redeem command.",
      "triggers": ["Twitch -> Channel Reward -> Reward Redemption (Captain Stretch reward)"],
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
      "mixItUpCommandIds": ["1facdbe6-f292-4d1b-9b66-ad71ae6de310"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_captain_stretch", "captain_stretch_thank_count"],
      "runtimeBehavior": [
        "Read outgoing captain from current_captain_stretch.",
        "If outgoing captain's thank count exceeds persisted high score, announce new record in chat and update high score vars.",
        "Write new captain username to current_captain_stretch.",
        "Reset captain_stretch_thank_count to 0.",
        "Reset captain_stretch_stretch_next_allowed_utc and captain_stretch_shrimp_next_allowed_utc so new captain starts with no cooldown debt.",
        "Fire Mix It Up command with new captain username as Arguments, SpecialIdentifiers.user, and SpecialIdentifiers.commander."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Role assignment still completes.",
        "Return true after slot update."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for Captain Stretch channel-point redeem"
    },
    {
      "script": "captain-stretch-thank.cs",
      "action": "Commanders - Captain Stretch - Thank",
      "purpose": "Handles public !thank support calls for the current Captain Stretch. Increments the per-tenure thank count. Blocks self-support.",
      "triggers": ["Twitch -> Chat Command -> !thank"],
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
      "requiredLiterals": ["current_captain_stretch", "captain_stretch_thank_count"],
      "runtimeBehavior": [
        "Read current_captain_stretch defensively.",
        "If no active Captain Stretch, send guidance message and return.",
        "If caller is current Captain Stretch, block self-support and return.",
        "Increment captain_stretch_thank_count by 1.",
        "Send success message in chat."
      ],
      "failureBehavior": [
        "Missing or blank current_captain_stretch treated as no active captain.",
        "Return true after guidance or success message."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !thank"
    },
    {
      "script": "captain-stretch-stretch.cs",
      "action": "Commanders - Captain Stretch - Stretch",
      "purpose": "Handles Captain Stretch-only !stretch command. Validates input (0-10 words, max 40 chars), enforces 5-minute cooldown, fires Mix It Up command.",
      "triggers": ["Twitch -> Chat Command -> !stretch"],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_stretch_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["60b43da9-accb-4dbe-968a-d57846a7dc4c"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_captain_stretch", "captain_stretch_stretch_next_allowed_utc"],
      "runtimeBehavior": [
        "Read current_captain_stretch defensively.",
        "If caller is not current Captain Stretch: if slot active, send !thank instruction; else encourage redeem.",
        "If caller is Captain Stretch and on cooldown, send cooldown remaining message.",
        "Validate input: 0 to 10 words and max 40 characters (reads rawInput, fallback message).",
        "Fire Mix It Up command with validated stretch phrase as Arguments.",
        "Refresh captain_stretch_stretch_next_allowed_utc to now + 5 minutes."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !stretch"
    },
    {
      "script": "captain-stretch-shrimp.cs",
      "action": "Commanders - Captain Stretch - Shrimp",
      "purpose": "Handles Captain Stretch-only !shrimp command. Validates input (0-30 words), enforces 1-minute cooldown, fires Mix It Up command.",
      "triggers": ["Twitch -> Chat Command -> !shrimp"],
      "globals": [
        "current_captain_stretch",
        "captain_stretch_shrimp_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["af5567d1-ac94-49bf-ad7b-0b7e034cb05d"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_captain_stretch", "captain_stretch_shrimp_next_allowed_utc"],
      "runtimeBehavior": [
        "Read current_captain_stretch defensively.",
        "If caller is not current Captain Stretch: if slot active, send !thank instruction; else encourage redeem.",
        "If caller is Captain Stretch and on cooldown, send cooldown remaining message.",
        "Validate input: 0 to 30 words (reads rawInput, fallback message).",
        "Fire Mix It Up command with validated shrimp phrase as Arguments.",
        "Refresh captain_stretch_shrimp_next_allowed_utc to now + 1 minute."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !shrimp"
    },
    {
      "script": "captain-stretch-generalfocus.cs",
      "action": "Commanders - Captain Stretch - General Focus",
      "purpose": "Handles Captain Stretch-only !generalfocus X command during the rest/focus loop's pre_focus window. Sets the operator-selected focus duration and arms the focus timer.",
      "triggers": ["Twitch -> Chat Command -> !generalfocus"],
      "globals": [
        "current_captain_stretch",
        "rest_focus_loop_active",
        "rest_focus_loop_phase"
      ],
      "timers": ["Rest Focus - Focus", "Rest Focus - Rest", "Rest Focus - Pre Focus", "Rest Focus - Pre Rest"],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": [
        "current_captain_stretch",
        "rest_focus_loop_active",
        "rest_focus_loop_phase",
        "pre_focus",
        "focus",
        "Rest Focus - Focus"
      ],
      "runtimeBehavior": [
        "Read current_captain_stretch defensively.",
        "If caller is not current Captain Stretch: if slot active, send !thank instruction; else explain default focus duration will be used.",
        "If loop is not active, send short message that crew is not in a rest/focus loop.",
        "If loop is not in pre_focus phase, send short message that focus window is not open.",
        "Parse X as whole minutes 1-999 from input0, fallback rawInput/message.",
        "Set rest_focus_loop_phase = 'focus' before arming timer.",
        "Fire Mix It Up placeholder command with duration in seconds as Arguments and SpecialIdentifiers.time.",
        "Disable all non-target rest/focus loop timers.",
        "Disable and re-arm timer 'Rest Focus - Focus' with X minutes converted to seconds.",
        "If timer arming fails, disable all four loop timers and set rest_focus_loop_active = false."
      ],
      "failureBehavior": [
        "Mix It Up failure does not stop the loop.",
        "Timer-arm failure disables all four rest/focus timers and clears rest_focus_loop_active.",
        "Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !generalfocus"
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

- Do not rename `current_captain_stretch`, `captain_stretch_thank_count`, or high-score keys without explicit operator request.
- Do not change Captain Stretch character identity or public copy without `brand-steward` review.
- Do not migrate The Director, Water Wizard, Squad, Voice Commands, LotAT, or overlay behavior into this guide.

## Handoff Notes

After changes, follow:

- [change-summary](../../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files. Operator must manually paste changed script contents into matching Streamer.bot actions and verify trigger wiring.

Brand handoff triggers: public chat copy, reward wording, command help text, Captain Stretch identity, or character/lore changes.

---

## Script Reference

### Script: `captain-stretch-redeem.cs`

#### Purpose
Assigns the current Captain Stretch commander slot occupant, finalizes the outgoing captain's `!thank` score, and checks for a new all-time high score.

#### Expected Trigger / Input
- Redeem/action trigger for Captain Stretch role assignment.
- Reads `user`.

#### Required Runtime Variables
- Reads/writes `current_captain_stretch`.
- Reads/writes `captain_stretch_thank_count`.
- Reads/writes `captain_stretch_stretch_next_allowed_utc`.
- Reads/writes `captain_stretch_shrimp_next_allowed_utc`.
- Reads/writes (persisted) `captain_stretch_thank_high_score`.
- Reads/writes (persisted) `captain_stretch_thank_high_score_user`.

#### Key Outputs / Side Effects
- Updates active Captain Stretch commander assignment.
- Resets `captain_stretch_thank_count` to `0` for the new captain tenure.
- Resets all Captain Stretch command cooldown vars so the new captain starts with no cooldown debt.
- If outgoing captain beat the high score, announces the new record in chat.
- Triggers Mix It Up command `Commander - Captain Stretch - Redeem` after the new captain is stored.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `1facdbe6-f292-4d1b-9b66-ad71ae6de310`
- Payload `Arguments`: new Captain Stretch username
- Payload `SpecialIdentifiers.user`: new Captain Stretch username
- Payload `SpecialIdentifiers.commander`: new Captain Stretch username

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Announces new high score when record is beaten.

#### Operator Notes
- Keep variable key `current_captain_stretch` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

### Script: `captain-stretch-thank.cs`

#### Purpose
Handles public `!thank` support calls for the current Captain Stretch.

#### Expected Trigger / Input
- Chat command/action trigger for `!thank`.
- Reads `user`.

#### Required Runtime Variables
- Reads `current_captain_stretch`.
- Reads/writes `captain_stretch_thank_count`.

#### Key Outputs / Side Effects
- Increments `captain_stretch_thank_count` by 1 per valid `!thank`.
- Blocks self-support (current Captain Stretch cannot `!thank` themselves).
- If no active Captain Stretch exists, responds with guidance.

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

### Script: `captain-stretch-stretch.cs`

#### Purpose
Handles Captain Stretch-only `!stretch` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!stretch`.
- Allows optional text after the command, up to `10` words and `40` characters max (reads `rawInput`, then fallback `message`).

#### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_stretch_next_allowed_utc` (Unix timestamp, UTC, used for 5-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the stretch phrase (0 to 10 words, max 40 characters) as payload `Arguments`.
  - Starts/refreshes 5-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `60b43da9-accb-4dbe-968a-d57846a7dc4c`
- Payload `Arguments`: the validated stretch phrase (0 to 10 words, max 40 characters)

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Wire this script to the `!stretch` command trigger action.

---

### Script: `captain-stretch-shrimp.cs`

#### Purpose
Handles Captain Stretch-only `!shrimp` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!shrimp`.
- Allows optional text after the command, up to `30` words max (reads `rawInput`, then fallback `message`).

#### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads/writes `captain_stretch_shrimp_next_allowed_utc` (Unix timestamp, UTC, used for 1-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and phrase is valid and off cooldown:
  - Triggers Mix It Up command and forwards the shrimp phrase (up to 30 words) as payload `Arguments`.
  - Starts/refreshes 1-minute cooldown.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, encourages caller to redeem and become Captain Stretch.
- If caller **is** current Captain Stretch but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `af5567d1-ac94-49bf-ad7b-0b7e034cb05d`
- Payload `Arguments`: the validated shrimp phrase (up to 30 words)

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Wire this script to the `!shrimp` command trigger action.

---

### Script: `captain-stretch-generalfocus.cs`

#### Purpose
Handles Captain Stretch-only `!generalfocus X` command usage during the rest/focus loop's pre-focus window.

#### Expected Trigger / Input
- Chat command/action trigger for `!generalfocus`.
- Expects `X` to be a whole number of minutes from `1` to `999` (reads `input0` first, then falls back to `rawInput` / `message`).

#### Required Runtime Variables
- Reads `current_captain_stretch` (active Captain Stretch username).
- Reads `rest_focus_loop_active`.
- Reads/writes `rest_focus_loop_phase`.

#### Key Outputs / Side Effects
- If caller **is** current Captain Stretch and the loop is in `pre_focus`:
  - Sets `rest_focus_loop_phase = "focus"` before arming the next timer so overlapping triggers see the intended target state.
  - Triggers Mix It Up placeholder command `Captain's Focus` with `Arguments = seconds` and `SpecialIdentifiers.time = seconds`.
  - Defensively disables every non-target rest/focus loop timer, then disables and re-arms timer `Rest Focus - Focus` using `X` minutes converted to seconds.
  - If timer arming fails, disables all four loop timers and marks `rest_focus_loop_active = false` so the operator can restart cleanly.
- If caller **is not** current Captain Stretch:
  - If a Captain Stretch is active, sends Twitch chat instruction to type `!thank`.
  - If no Captain Stretch is active, explains that the default focus duration will be used instead.
- If caller **is** current Captain Stretch but the loop is not active:
  - Sends a short chat message that the crew is not in a rest/focus loop right now.
- If caller **is** current Captain Stretch but the loop is not in `pre_focus`:
  - Sends a short chat message explaining that the focus window is not open right now.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID` *(placeholder; must be replaced)*
- Payload `Arguments`: focus duration in seconds
- Payload `SpecialIdentifiers.time`: focus duration in seconds

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends short guidance messages for unauthorized use, invalid usage, inactive loop state, and wrong loop phase.
- Logs warning/error if Mix It Up call fails.
- Logs timer arming attempts, timer-arm failures, and fail-closed loop recovery actions.

#### Operator Notes
- Replace `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID` before production use.
- Wire this script to the `!generalfocus` command trigger action.
- This script depends on `CPH.SetTimerInterval(string, int)` to apply the operator-selected focus duration. Verify that method in your Streamer.bot build before production use.
- Mix It Up failure does **not** stop the loop. Timer-arm failure does.
- If timer arming fails, the script disables all four rest/focus timers and clears `rest_focus_loop_active`. Recovery is manual: fix the timer/setup problem, then restart the loop from its normal start action.
