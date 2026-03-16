# Commanders Script Reference

## Scope
This folder contains commander slot assignment scripts and commander support command scripts.

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

## Commander Docs
- `Captain Stretch/README.md`
- `The Director/README.md`
- `Water Wizard/README.md`

## Shared Constants
- Cross-script key sync reference: `Actions/SHARED-CONSTANTS.md`

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
