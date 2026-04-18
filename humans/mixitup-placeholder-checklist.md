# Mix It Up Placeholder Checklist

Last updated: 2026-04-18
Source of truth: `Tools/MixItUp/Api/data/mixitup-commands.txt`

Rule used for this checklist:
- If a Mix It Up command ID is **not** present in `Tools/MixItUp/Api/data/mixitup-commands.txt`, it is treated as **not set up**.
- Scripts should use either:
  - the exact exported ID from that file, or
  - a clear `REPLACE_WITH_...` placeholder until the Mix It Up action group exists.

## How to work this list

For each item below:
1. Create or confirm the matching Mix It Up action group exists.
2. Refresh `Tools/MixItUp/Api/data/mixitup-commands.txt`.
3. Copy the exported ID into the listed `Actions/*.cs` file.
4. If the feature has a matching `README.md`, update that too.
5. Paste the updated script into Streamer.bot.

---

## Captain Stretch

- [ ] `Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs`
  - Constant: `MIXITUP_CAPTAINS_FOCUS_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID`

## The Director

All current placeholder entries in this section have been resolved.

## Water Wizard

- [ ] `Actions/Commanders/Water Wizard/water-wizard-castrest.cs`
  - Constant: `MIXITUP_WIZARDS_REST_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_WIZARDS_REST_COMMAND_ID`

## Rest / Focus Loop

- [ ] `Actions/Rest Focus Loop/rest-focus-pre-rest-end.cs`
  - Constant: `MIXITUP_WIZARDS_REST_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_WIZARDS_REST_COMMAND_ID`

- [ ] `Actions/Rest Focus Loop/rest-focus-rest-end.cs`
  - Constant: `MIXITUP_CAPTAINS_FOCUS_WARNING_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CAPTAINS_FOCUS_WARNING_COMMAND_ID`

- [ ] `Actions/Rest Focus Loop/rest-focus-pre-focus-end.cs`
  - Constant: `MIXITUP_CAPTAINS_FOCUS_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CAPTAINS_FOCUS_COMMAND_ID`

- [ ] `Actions/Rest Focus Loop/rest-focus-focus-end.cs`
  - Constant: `MIXITUP_FOCUS_TIMER_END_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_FOCUS_TIMER_END_COMMAND_ID`

## Squad

All current placeholder entries in this section have been resolved.

## Temporary

- [ ] `Actions/Temporary/temp-focus-timer-start.cs`
  - Constant: `MIXITUP_TEMPORARY_LOCK_IN_TIMER_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_TEMPORARY_LOCK_IN_TIMER_COMMAND_ID`

- [ ] `Actions/Temporary/temp-focus-timer-end.cs`
  - Constant: `MIXITUP_CAPTAIN_STRETCH_LOCK_IN_TIMER_END_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CAPTAIN_STRETCH_LOCK_IN_TIMER_END_COMMAND_ID`

## Twitch Bits Integrations

- [ ] `Actions/Twitch Bits Integrations/gigantify-emote.cs`
  - Constant: `MIXITUP_GIGANTIFY_EMOTE_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_GIGANTIFY_EMOTE_COMMAND_ID`

- [ ] `Actions/Twitch Bits Integrations/message-effects.cs`
  - Constant: `MIXITUP_MESSAGE_EFFECTS_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_MESSAGE_EFFECTS_COMMAND_ID`

- [ ] `Actions/Twitch Bits Integrations/on-screen-celebration.cs`
  - Constant: `MIXITUP_ON_SCREEN_CELEBRATION_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_ON_SCREEN_CELEBRATION_COMMAND_ID`

## Twitch Core Integrations

- [ ] `Actions/Twitch Core Integrations/follower-new.cs`
  - Constant: `MIXITUP_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CORE_FOLLOWER_NEW_COMMAND_ID`

- [ ] `Actions/Twitch Core Integrations/subscription-counter-rollover.cs`
  - Constant: `MIXITUP_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_CORE_SUBSCRIPTION_COUNTER_ROLLOVER_COMMAND_ID`

## Twitch Hype Train

- [ ] `Actions/Twitch Hype Train/hype-train-start.cs`
  - Constant: `MIXITUP_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_HYPE_TRAIN_START_COMMAND_ID`

- [ ] `Actions/Twitch Hype Train/hype-train-level-up.cs`
  - Constant: `MIXITUP_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_HYPE_TRAIN_LEVEL_UP_COMMAND_ID`

- [ ] `Actions/Twitch Hype Train/hype-train-end.cs`
  - Constant: `MIXITUP_COMMAND_ID`
  - Placeholder: `REPLACE_WITH_HYPE_TRAIN_END_COMMAND_ID`

---

## Already matched to current export

These do **not** need placeholder work right now because their IDs exist in `Tools/MixItUp/Api/data/mixitup-commands.txt`:

- `Actions/Squad/Pedro/pedro-main.cs`
- `Actions/Squad/Pedro/pedro-resolve.cs`
- `Actions/Squad/Duck/duck-call.cs`
- `Actions/Squad/Clone/clone-volley.cs`
- `Actions/Squad/Toothless/toothless-main.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-stretch.cs`
- `Actions/Commanders/Captain Stretch/captain-stretch-shrimp.cs`
- `Actions/Commanders/The Director/the-director-redeem.cs`
- `Actions/Commanders/The Director/the-director-checkchat.cs`
- `Actions/Commanders/The Director/the-director-toad.cs`
- `Actions/Commanders/Water Wizard/water-wizard-redeem.cs`
- `Actions/Commanders/Water Wizard/wizard-hydrate.cs`
- `Actions/Commanders/Water Wizard/water-wizard-orb.cs`
- `Actions/Twitch Core Integrations/watch-streak.cs`
- `Actions/Twitch Core Integrations/subscription-new.cs`
- `Actions/Twitch Core Integrations/subscription-renewed.cs`
- `Actions/Twitch Core Integrations/subscription-prime-paid-upgrade.cs`
- `Actions/Twitch Core Integrations/subscription-gift-paid-upgrade.cs`
- `Actions/Twitch Core Integrations/subscription-pay-it-forward.cs`
- `Actions/Twitch Core Integrations/subscription-gift.cs`
- `Actions/Twitch Bits Integrations/bits-tier-1.cs`
- `Actions/Twitch Bits Integrations/bits-tier-2.cs`
- `Actions/Twitch Bits Integrations/bits-tier-3.cs`
- `Actions/Twitch Bits Integrations/bits-tier-4.cs`
