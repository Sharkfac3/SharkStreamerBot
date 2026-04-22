# Commanders Script Reference

## Scope
This folder contains commander slot assignment scripts, commander support command scripts, and shared commander helper scripts.

## Model Rules
- Three commander slots exist:
  - Captain Stretch
  - The Director
  - Water Wizard
- All three commander slots can be active simultaneously.
- Redeem behavior should remain backward-compatible unless intentionally changed.

## Support Command Rules
- Chat can support active commanders with:
  - `!hail` (Water Wizard)
  - `!thank` (Captain Stretch)
  - `!award` (The Director)
- Active commander cannot support themselves with their own support command.
- Each support command increments a per-tenure counter.
- On commander redeem, outgoing tenure counter is compared to persistent high score for that role.

## Commander-Only Command Rules
- Water Wizard can run `!hydrate`, `!orb`, and `!castrest` when the relevant feature window is active.
- Captain Stretch can run `!stretch`, `!shrimp`, and `!generalfocus` when the relevant feature window is active.
- The Director can run `!checkchat`, `!toad`, `!primary`, and `!secondary`.
- Unauthorized callers should get short guidance that points them back to the active commander support command.
- New loop-control commands must preserve the existing commander assignment model.

## Commander Docs
- `Captain Stretch/README.md`
- `The Director/README.md`
- `Water Wizard/README.md`

## Shared Commander Helper Scripts
- `commander-help.cs` — chat-facing helper that tells the active caller which commander-only actions they can use for their current role(s).

## Shared Constants
- Cross-script key sync reference: `Actions/SHARED-CONSTANTS.md`

---

## Script: `commander-help.cs`

### Purpose
Gives the caller a short, commander-specific help message in chat.

### Expected Trigger / Input
- Chat command or action trigger for a commander help command (operator chooses the exact command name, such as `!commanderhelp`).
- Reads `user`.

### Required Runtime Variables
- Reads `current_captain_stretch`.
- Reads `current_the_director`.
- Reads `current_water_wizard`.

### Key Outputs / Side Effects
- If caller is the active Captain Stretch, explains `!stretch` and `!shrimp`.
- If caller is the active The Director, explains `!checkchat`, `!toad`, `!primary`, and `!secondary`.
- If caller is the active Water Wizard, explains `!hydrate` and `!orb`.
- If caller holds multiple commander roles, sends one short help message for each matching role.
- If caller is not an active commander, sends a short guidance message telling them to redeem first.

### Mix It Up Actions
- None.

### OBS Interactions
- None directly.

### Wait Behavior
- None.

### Chat / Log Output
- Sends short role-specific command summaries in chat.
- Sends a short fallback guidance message for non-commanders.

### Operator Notes
- Wire this script to the chat command name you want to use.
- This script is read-only: it does not create or change any global variables.

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Channel Reward Redemption (commander role redeems)

Commander role assignment is triggered via Twitch → Channel Reward → Reward Redemption.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the redeeming user — becomes the new commander |
| `userId` | string | Twitch user ID |
| `rewardName` | string | Name of the channel point reward |
| `rewardId` | string | Unique reward identifier |
| `rawInput` | string | Optional user text input (if the reward prompts for it) |

### Chat Message (support commands: !thank, !award, !hail)

Support commands are triggered via Twitch → Chat → Message or a Command trigger.

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user running the command |
| `userId` | string | Twitch user ID |
| `message` | string | Full chat message |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection |
