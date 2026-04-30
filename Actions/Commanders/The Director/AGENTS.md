---
id: actions-commanders-the-director
type: domain-route
description: The Director commander slot scripts, redeem behavior, !award support command, and commander-only commands.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# The Director — Agent Guide

## Purpose

This folder owns the Streamer.bot C# scripts for The Director commander slot: role assignment via channel-point redeem, `!award` public support command, and The Director-only commands (`!checkchat`, `!toad`, `!primary`, `!secondary`).

## When to Activate

Use this guide when editing or reviewing any file under [Actions/Commanders/The Director/](./).

Activate the `brand-steward` role before changing public chat copy, command names presented to chat, or The Director character identity.

## Primary Owner

`streamerbot-dev` owns C# runtime behavior, paste readiness, global variable use, and command wiring expectations.

## Secondary Owners / Chain To

- `brand-steward` — chain for public chat text, The Director character voice, reward descriptions, or naming changes.
- `ops` — chain for validation, paste/sync workflow, or agent-tree maintenance.

## Required Reading

- [Actions/Commanders/AGENTS.md](../AGENTS.md) — parent commander guide and shared commander slot model rules.
- [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md)
- [Actions/Helpers/AGENTS.md](../../Helpers/AGENTS.md)
- [Creative/Brand/BRAND-VOICE.md](../../../Creative/Brand/BRAND-VOICE.md) when public copy changes.
- [Creative/Brand/CHARACTER-CODEX.md](../../../Creative/Brand/CHARACTER-CODEX.md) when The Director characterization changes.

## Local Workflow

1. Read the parent [Actions/Commanders/AGENTS.md](../AGENTS.md) for the shared commander slot model and support command rules.
2. Preserve The Director slot model — one active slot, backward-compatible redeem behavior.
3. Do not rename `current_the_director`, `the_director_award_count`, or high-score keys without explicit operator request.
4. Read runtime state via `CPH.TryGetArg` or Streamer.bot globals defensively; protect against missing or malformed inputs.
5. Keep scripts self-contained and paste-ready. Do not assume shared runtime files can be imported by Streamer.bot actions.
6. Update the Action Contracts and Script Reference sections in this file when trigger variables, command behavior, state variables, or operator wiring changes.
7. If a new global variable, OBS source, timer, or command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md) or flag it in the handoff.
8. Keep `SCENE_SOURCE_MAP` in `the-director-primary.cs` and `the-director-secondary.cs` in sync when scene/source entries change.

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "the-director-redeem.cs",
      "action": "Commanders - The Director - Redeem",
      "purpose": "Assigns the current The Director slot, finalizes the outgoing director's !award score, checks for a new all-time high score, and fires the Mix It Up redeem command.",
      "triggers": ["Twitch -> Channel Reward -> Reward Redemption (The Director reward)"],
      "globals": [
        "current_the_director",
        "the_director_award_count",
        "the_director_checkchat_next_allowed_utc",
        "the_director_toad_next_allowed_utc",
        "the_director_award_high_score",
        "the_director_award_high_score_user"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["51146998-c2bc-4f46-b6a8-13069565a562"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_the_director", "the_director_award_count"],
      "runtimeBehavior": [
        "Read outgoing director from current_the_director.",
        "If outgoing director's award count exceeds persisted high score, announce new record in chat and update high score vars.",
        "Write new director username to current_the_director.",
        "Reset the_director_award_count to 0.",
        "Reset the_director_checkchat_next_allowed_utc and the_director_toad_next_allowed_utc so new director starts with no cooldown debt.",
        "Fire Mix It Up command with new director username as Arguments, SpecialIdentifiers.user, and SpecialIdentifiers.commander."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Role assignment still completes.",
        "Return true after slot update."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for The Director channel-point redeem"
    },
    {
      "script": "the-director-award.cs",
      "action": "Commanders - The Director - Award",
      "purpose": "Handles public !award support calls for the current The Director. Increments the per-tenure award count. Blocks self-support.",
      "triggers": ["Twitch -> Chat Command -> !award"],
      "globals": [
        "current_the_director",
        "the_director_award_count"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": ["current_the_director", "the_director_award_count"],
      "runtimeBehavior": [
        "Read current_the_director defensively.",
        "If no active Director, send guidance message and return.",
        "If caller is current The Director, block self-support and return.",
        "Increment the_director_award_count by 1.",
        "Send success message in chat."
      ],
      "failureBehavior": [
        "Missing or blank current_the_director treated as no active director.",
        "Return true after guidance or success message."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !award"
    },
    {
      "script": "the-director-checkchat.cs",
      "action": "Commanders - The Director - Check Chat",
      "purpose": "Handles The Director-only !checkchat command. Accepts optional text (0-20 words, max 40 chars), enforces 1-minute cooldown, fires Mix It Up command.",
      "triggers": ["Twitch -> Chat Command -> !checkchat"],
      "globals": [
        "current_the_director",
        "the_director_checkchat_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["06e3851f-81a2-40cb-a911-33c5ec04a3f2"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_the_director", "the_director_checkchat_next_allowed_utc"],
      "runtimeBehavior": [
        "Read current_the_director defensively.",
        "If caller is not current The Director: if slot active, send !award instruction; else encourage redeem.",
        "If caller is The Director and on cooldown, send cooldown remaining message.",
        "Validate input: 0 to 20 words and max 40 characters (reads rawInput, fallback message). !checkchat by itself is valid.",
        "Fire Mix It Up command with validated checkchat text as Arguments.",
        "Refresh the_director_checkchat_next_allowed_utc to now + 1 minute."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !checkchat"
    },
    {
      "script": "the-director-toad.cs",
      "action": "Commanders - The Director - Toad",
      "purpose": "Handles The Director-only !toad command. Accepts optional text (0-30 words), enforces 1-minute cooldown, fires Mix It Up command with a 1-in-10 chance of 'hypno' type.",
      "triggers": ["Twitch -> Chat Command -> !toad"],
      "globals": [
        "current_the_director",
        "the_director_toad_next_allowed_utc"
      ],
      "timers": [],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": ["5440fa4e-b84e-438a-a409-f398b637f3e7"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_the_director", "the_director_toad_next_allowed_utc", "normal", "hypno"],
      "runtimeBehavior": [
        "Read current_the_director defensively.",
        "If caller is not current The Director: if slot active, send !award instruction; else encourage redeem.",
        "If caller is The Director and on cooldown, send cooldown remaining message.",
        "Validate input: 0 to 30 words (reads rawInput, fallback message). !toad by itself is valid.",
        "Set SpecialIdentifiers.type = 'normal' by default; 1-in-10 chance of 'hypno'.",
        "Fire Mix It Up command with validated toad text as Arguments and SpecialIdentifiers.type.",
        "Refresh the_director_toad_next_allowed_utc to now + 1 minute."
      ],
      "failureBehavior": [
        "If Mix It Up call fails, log warning/error. Cooldown is still set.",
        "Return true after any branch."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !toad"
    },
    {
      "script": "the-director-primary.cs",
      "action": "Commanders - The Director - Primary",
      "purpose": "Lets the active The Director switch the current OBS scene to its primary source layout. Shows the primary source, hides the secondary source for the active scene.",
      "triggers": ["Twitch -> Chat Command -> !primary"],
      "globals": [
        "current_the_director"
      ],
      "timers": [],
      "obsSources": ["Main Screen Capture", "Quest POV"],
      "obsScenes": ["Workspace: Main"],
      "mixItUpCommandIds": ["REPLACE_WITH_PRIMARY_COMMAND_ID"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_the_director", "primary", "Workspace: Main", "Main Screen Capture", "Quest POV"],
      "runtimeBehavior": [
        "Read current_the_director defensively.",
        "If caller is not current The Director, log warn and return.",
        "Call ObsGetCurrentScene() to get active scene.",
        "Look up scene in SCENE_SOURCE_MAP; if unmapped, log warn and return.",
        "Call ObsShowSource(scene, primarySource) and ObsHideSource(scene, secondarySource).",
        "If Mix It Up command ID is configured (not placeholder), fire command with Arguments = 'primary' and SpecialIdentifiers.state = 'primary'.",
        "Log warn on success showing which source was shown/hidden."
      ],
      "failureBehavior": [
        "Guard exits (wrong caller, no Director, unmapped scene) log warn and return without OBS changes.",
        "Mix It Up not active until REPLACE_WITH_PRIMARY_COMMAND_ID is replaced."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !primary"
    },
    {
      "script": "the-director-secondary.cs",
      "action": "Commanders - The Director - Secondary",
      "purpose": "Lets the active The Director switch the current OBS scene to its secondary source layout. Shows the secondary source, hides the primary source for the active scene.",
      "triggers": ["Twitch -> Chat Command -> !secondary"],
      "globals": [
        "current_the_director"
      ],
      "timers": [],
      "obsSources": ["Main Screen Capture", "Quest POV"],
      "obsScenes": ["Workspace: Main"],
      "mixItUpCommandIds": ["REPLACE_WITH_SECONDARY_COMMAND_ID"],
      "overlayTopics": [],
      "serviceUrls": ["http://localhost:8911/api/v2/commands/{commandId}"],
      "requiredLiterals": ["current_the_director", "secondary", "Workspace: Main", "Main Screen Capture", "Quest POV"],
      "runtimeBehavior": [
        "Read current_the_director defensively.",
        "If caller is not current The Director, log warn and return.",
        "Call ObsGetCurrentScene() to get active scene.",
        "Look up scene in SCENE_SOURCE_MAP; if unmapped, log warn and return.",
        "Call ObsShowSource(scene, secondarySource) and ObsHideSource(scene, primarySource).",
        "If Mix It Up command ID is configured (not placeholder), fire command with Arguments = 'secondary' and SpecialIdentifiers.state = 'secondary'.",
        "Log warn on success showing which source was shown/hidden."
      ],
      "failureBehavior": [
        "Guard exits (wrong caller, no Director, unmapped scene) log warn and return without OBS changes.",
        "Mix It Up not active until REPLACE_WITH_SECONDARY_COMMAND_ID is replaced."
      ],
      "pasteTarget": "Streamer.bot Execute C# Code action for !secondary"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Validation

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no external imports beyond what Streamer.bot supports, no repo-only helper file dependencies.
- Verify all global names, OBS source/scene names, and Mix It Up command IDs against [Actions/SHARED-CONSTANTS.md](../../SHARED-CONSTANTS.md).
- Keep `SCENE_SOURCE_MAP` in `the-director-primary.cs` and `the-director-secondary.cs` in sync.
- Verify `ObsGetCurrentScene()` resolves in the current Streamer.bot build before shipping primary/secondary scripts.
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

- For action contract changes, run:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --changed
```

## Boundaries / Out of Scope

- Do not rename `current_the_director`, `the_director_award_count`, or high-score keys without explicit operator request.
- Do not change The Director character identity or public copy without `brand-steward` review.
- Do not migrate Captain Stretch, Water Wizard, Squad, Voice Commands, LotAT, or overlay behavior into this guide.

## Handoff Notes

After changes, follow:

- [change-summary](../../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files. Operator must manually paste changed script contents into matching Streamer.bot actions and verify trigger wiring.

Brand handoff triggers: public chat copy, reward wording, command help text, The Director identity, or character/lore changes.

---

## Script Reference

### Script: `the-director-redeem.cs`

#### Purpose
Assigns the current The Director commander slot occupant, finalizes the outgoing director's `!award` score, and checks for a new all-time high score.

#### Expected Trigger / Input
- Redeem/action trigger for The Director role assignment.
- Reads `user`.

#### Required Runtime Variables
- Reads/writes `current_the_director`.
- Reads/writes `the_director_award_count`.
- Reads/writes `the_director_checkchat_next_allowed_utc`.
- Reads/writes `the_director_toad_next_allowed_utc`.
- Reads/writes (persisted) `the_director_award_high_score`.
- Reads/writes (persisted) `the_director_award_high_score_user`.

#### Key Outputs / Side Effects
- Updates active The Director commander assignment.
- Resets `the_director_award_count` to `0` for the new director tenure.
- Resets all The Director command cooldown vars so the new director starts with no cooldown debt.
- If outgoing director beat the high score, announces the new record in chat.
- Triggers Mix It Up command `Commander - The Director - Redeem` after the new director is stored.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `51146998-c2bc-4f46-b6a8-13069565a562`
- Payload `Arguments`: new The Director username
- Payload `SpecialIdentifiers.user`: new The Director username
- Payload `SpecialIdentifiers.commander`: new The Director username

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Announces new high score when record is beaten.

#### Operator Notes
- Keep variable key `current_the_director` unchanged to preserve existing integrations.
- High score vars are intentionally persisted across Streamer.bot restarts.

---

### Script: `the-director-award.cs`

#### Purpose
Handles public `!award` support calls for the current The Director.

#### Expected Trigger / Input
- Chat command/action trigger for `!award`.
- Reads `user`.

#### Required Runtime Variables
- Reads `current_the_director`.
- Reads/writes `the_director_award_count`.

#### Key Outputs / Side Effects
- Increments `the_director_award_count` by 1 per valid `!award`.
- Blocks self-support (current The Director cannot `!award` themselves).
- If no active Director exists, responds with guidance.

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

### Script: `the-director-checkchat.cs`

#### Purpose
Handles The Director-only `!checkchat` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!checkchat`.
- Accepts up to `20` words and `40` characters after the command (reads `rawInput`, then fallback `message`).
- Message text is optional, so `!checkchat` by itself is valid.

#### Required Runtime Variables
- Reads `current_the_director` (active The Director username).
- Reads/writes `the_director_checkchat_next_allowed_utc` (Unix timestamp, UTC, used for 1-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current The Director and input is valid and off cooldown:
  - Triggers Mix It Up command and forwards the optional text (0 to 20 words, max 40 characters) as payload `Arguments`.
  - Starts/refreshes 1-minute cooldown.
- If caller **is not** current The Director:
  - If The Director is active, sends Twitch chat instruction to type `!award`.
  - If no The Director is active, encourages caller to redeem and become The Director.
- If caller **is** current The Director but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `06e3851f-81a2-40cb-a911-33c5ec04a3f2`
- Payload `Arguments`: validated `!checkchat` text (optional, max 20 words and 40 characters)

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to the `!checkchat` command trigger action.

---

### Script: `the-director-toad.cs`

#### Purpose
Handles The Director-only `!toad` command usage.

#### Expected Trigger / Input
- Chat command/action trigger for `!toad`.
- Accepts up to `30` words after the command (reads `rawInput`, then fallback `message`).
- Message text is optional, so `!toad` by itself is valid.

#### Required Runtime Variables
- Reads `current_the_director` (active The Director username).
- Reads/writes `the_director_toad_next_allowed_utc` (Unix timestamp, UTC, used for 1-minute cooldown).

#### Key Outputs / Side Effects
- If caller **is** current The Director and input is valid and off cooldown:
  - Triggers Mix It Up command and forwards the optional text (0 to 30 words) as payload `Arguments`.
  - Starts/refreshes 1-minute cooldown.
- If caller **is not** current The Director:
  - If The Director is active, sends Twitch chat instruction to type `!award`.
  - If no The Director is active, encourages caller to redeem and become The Director.
- If caller **is** current The Director but is on cooldown:
  - Sends cooldown remaining message in chat.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Command ID in script: `5440fa4e-b84e-438a-a409-f398b637f3e7`
- Payload `Arguments`: validated `!toad` text (optional, max 30 words)
- Payload `SpecialIdentifiers.type`:
  - Default value: `"normal"`
  - Random variant: `"hypno"` with a 1-in-10 chance

#### OBS Interactions
- None.

#### Wait Behavior
- None.

#### Chat / Log Output
- Sends chat guidance/feedback for unauthorized use, invalid usage, and cooldown status.
- Logs warning/error if Mix It Up call fails.

#### Operator Notes
- Current command ID is configured from the saved Mix It Up command export.
- Wire this script to the `!toad` command trigger action.

---

### Script: `the-director-primary.cs`

#### Purpose
Lets the active The Director switch the current OBS scene to its primary source layout.

#### Expected Trigger / Input
- Streamer.bot action trigger for `!primary`.
- Reads `user`.

#### Required Runtime Variables
- Reads `current_the_director`.

#### Key Outputs / Side Effects
- Shows the primary source and hides the secondary source in the current OBS scene.
- Source pairs per scene are defined in `SCENE_SOURCE_MAP` inside the script.
- If Mix It Up command ID is configured, triggers the primary switch command.
- No chat output.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Placeholder: `REPLACE_WITH_PRIMARY_COMMAND_ID`
- Payload `Arguments`: `"primary"`
- Payload `SpecialIdentifiers.state`: `"primary"`
- **Not active until command ID is replaced.**

#### OBS Interactions
- `ObsGetCurrentScene()` — reads the active scene. **VERIFY:** unconfirmed method signature — test before relying on in production.
- `ObsShowSource(scene, primarySource)` / `ObsHideSource(scene, secondarySource)`

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warn on guard exits (wrong caller, no Director, unmapped scene).
- Logs warn on success showing which source was shown/hidden.

#### Operator Notes
- Wire `!primary` action → run this script. No action argument needed.
- Add scene→source entries to `SCENE_SOURCE_MAP` inside the script as sources are confirmed.
- Keep `SCENE_SOURCE_MAP` in sync with `the-director-secondary.cs`.
- Currently mapped: `Workspace: Main` (primary: `Main Screen Capture`, secondary: `Quest POV`).
- Replace `REPLACE_WITH_PRIMARY_COMMAND_ID` when Mix It Up command exists.
- Verify `ObsGetCurrentScene()` resolves in Streamer.bot before shipping.

---

### Script: `the-director-secondary.cs`

#### Purpose
Lets the active The Director switch the current OBS scene to its secondary source layout.

#### Expected Trigger / Input
- Streamer.bot action trigger for `!secondary`.
- Reads `user`.

#### Required Runtime Variables
- Reads `current_the_director`.

#### Key Outputs / Side Effects
- Shows the secondary source and hides the primary source in the current OBS scene.
- Source pairs per scene are defined in `SCENE_SOURCE_MAP` inside the script.
- If Mix It Up command ID is configured, triggers the secondary switch command.
- No chat output.

#### Mix It Up Actions
- Endpoint: `POST http://localhost:8911/api/v2/commands/{commandId}`
- Placeholder: `REPLACE_WITH_SECONDARY_COMMAND_ID`
- Payload `Arguments`: `"secondary"`
- Payload `SpecialIdentifiers.state`: `"secondary"`
- **Not active until command ID is replaced.**

#### OBS Interactions
- `ObsGetCurrentScene()` — reads the active scene. **VERIFY:** unconfirmed method signature — test before relying on in production.
- `ObsShowSource(scene, secondarySource)` / `ObsHideSource(scene, primarySource)`

#### Wait Behavior
- None.

#### Chat / Log Output
- No chat output.
- Logs warn on guard exits (wrong caller, no Director, unmapped scene).
- Logs warn on success showing which source was shown/hidden.

#### Operator Notes
- Wire `!secondary` action → run this script. No action argument needed.
- Add scene→source entries to `SCENE_SOURCE_MAP` inside the script as sources are confirmed.
- Keep `SCENE_SOURCE_MAP` in sync with `the-director-primary.cs`.
- Currently mapped: `Workspace: Main` (primary: `Main Screen Capture`, secondary: `Quest POV`).
- Replace `REPLACE_WITH_SECONDARY_COMMAND_ID` when Mix It Up command exists.
- Verify `ObsGetCurrentScene()` resolves in Streamer.bot before shipping.
