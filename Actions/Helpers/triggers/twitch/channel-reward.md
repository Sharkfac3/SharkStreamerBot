---
id: triggers-twitch-channel-reward
type: reference
description: Streamer.bot Twitch Channel Reward trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded
upstream: https://docs.streamer.bot/api/triggers/twitch/channel-reward
---

# Twitch — Channel Reward Triggers

## Automatic Reward Redemption

- Path: Twitch -> Channel Reward -> Automatic Reward Redemption
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-reward/automatic-reward-redemption
- Min SB version: v0.2.4
- Shared groups: General, Twitch Broadcaster, Twitch Chat, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `gigantifiedEmoteId` | string | ID of the gigantified emote (not available for all reward types). |
| `gigantifiedEmoteName` | string | Name of the gigantified emote (not available for all reward types). |
| `gigantifiedEmoteUrl` | string | Image URL of the gigantified emote (not available for all reward types). |
| `rewardCost` | int | Channel point cost of the reward. |
| `rewardType` | string | Type of reward: `send_highlighted_message`, `random_sub_emote_unlock`, `chosen_sub_emote_unlock`, `chosen_modified_sub_emote_unlock`, `single_message_bypass_sub_mode`, `message_effect`, `gigantify_an_emote`, `celebration`. |
| `unlockedEmoteId` | string | ID of the unlocked emote (not available for all reward types). |
| `unlockedEmoteName` | string | Name of the unlocked emote (not available for all reward types). |
| `userInput` | string | Input message from the redeeming user (not available for all reward types). |

### Caveats

- Emote and userInput variables are only populated for applicable `rewardType` values.

### Used in repo

- [Actions/Twitch Bits Integrations/gigantify-emote.cs](../../Twitch%20Bits%20Integrations/gigantify-emote.cs)
- [Actions/Twitch Bits Integrations/message-effects.cs](../../Twitch%20Bits%20Integrations/message-effects.cs)
- [Actions/Twitch Bits Integrations/on-screen-celebration.cs](../../Twitch%20Bits%20Integrations/on-screen-celebration.cs)

---

## Reward Redemption

- Path: Twitch -> Channel Reward -> Reward Redemption
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-reward/reward-redemption
- Min SB version: v0.0.30
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `counter` | number | Number of times this reward has been used (all-time). |
| `rawInput` | string | Text entered by the user (if user input enabled on the reward). |
| `rawInputEscaped` | string | Escaped version of user text (if user input enabled). |
| `redemptionId` | string | Unique ID for this redemption. |
| `rewardCost` | number | Cost of the reward in channel points. |
| `rewardId` | string | Unique ID of the reward. |
| `rewardName` | string | Name of the reward. |
| `rewardPrompt` | string | Description of the reward. |
| `userCounter` | number | Number of times this user has redeemed this reward. |

### Caveats

- `rawInput` and `rawInputEscaped` only populate when user input is enabled for the reward.
- Requires selecting a specific reward in SB trigger config (or use "Any").
- EventSub-based trigger.

### Used in repo

- [Actions/Twitch Channel Points/disco-party.cs](../../Twitch%20Channel%20Points/disco-party.cs)
- [Actions/Twitch Channel Points/explain-current-task.cs](../../Twitch%20Channel%20Points/explain-current-task.cs)
- [Actions/Intros/redeem-capture.cs](../../Intros/redeem-capture.cs)

---

## Reward Redemption Updated

- Path: Twitch -> Channel Reward -> Reward Redemption Updated
- Upstream: https://docs.streamer.bot/api/triggers/twitch/channel-reward/reward-redemption-updated
- Min SB version: v0.2.0
- Shared groups: General, Twitch Broadcaster, Twitch User
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| `rawInputEscaped` | string | Escaped text entered by the user (if user input enabled). |
| `redemptionId` | string | Unique ID for this redemption. |
| `rewardCost` | number | Cost of the reward in channel points. |
| `rewardId` | string | Unique ID of the reward. |
| `rewardName` | string | Name of the reward. |
| `rewardPrompt` | string | Description of the reward. |
| `rewardStatus` | string | Whether the reward was marked as complete or rejected. |

### Caveats

- EventSub-based trigger.

### Used in repo

_Not yet wired._
