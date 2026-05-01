# Prompt 01 — Fix Custom Intro redemption trigger args

You are working in the SharkStreamerBot repository as `streamerbot-dev`.

## Task

Fix `Actions/Intros/redeem-capture.cs` so it consumes the actual Streamer.bot Twitch Channel Reward Redemption args documented in this repo.

Known gap:

- Current code reads `redeemId` and `rewardTitle`.
- `Actions/Helpers/triggers/twitch/channel-reward.md` documents the args as `redemptionId` and `rewardName`.
- Because `redeemId` is required by the script, the action likely no-ops today.

## Required reading before editing

1. `AGENTS.md`
2. `.agents/ENTRY.md`
3. `WORKING.md`
4. `.agents/roles/streamerbot-dev/role.md`
5. `Actions/AGENTS.md`
6. `Actions/Intros/AGENTS.md`
7. `Actions/Helpers/triggers/twitch/channel-reward.md`
8. `Apps/info-service/INFO-SERVICE-PLAN.md` sections for `pending-intros`

## Constraints

- Keep the change minimal and local.
- Do not change collection names, URLs, schemas, or public-facing reward copy.
- Preserve idempotency by redemption ID.
- Preserve stored JSON field names: `redeemId` and `rewardTitle` are still the info-service record fields unless an explicit schema migration is requested.
- Do not implement external Streamer.bot or Mix It Up setup.
- If you discover Streamer.bot provides both old and new arg names in this operator's version, ask before adding compatibility fallbacks.

## Implementation expectation

- Read `redemptionId` from Streamer.bot and store it in the existing local `redeemId` variable / JSON field.
- Read `rewardName` from Streamer.bot and store it in the existing local `rewardTitle` variable / JSON field.
- Keep `rawInput`, `userId`, and `userLogin` handling defensive.
- If validation would fail because required user/reward fields are missing, preserve safe no-op/log behavior or ask before changing failure behavior.

## Validation

Run targeted checks appropriate to the change:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/redeem-capture.cs"
```

If this fails only because Intros action contracts are missing, report that as the known repo-validation gap instead of solving it in this task.

Also review the script manually for Streamer.bot inline C# compatibility.

## Done criteria

- `redeem-capture.cs` consumes `redemptionId` / `rewardName` correctly.
- The generated `pending-intros` record shape remains compatible with `Apps/info-service/src/store/schemas/pending-intros.ts`.
- No unrelated refactor.
- Final handoff lists paste target, trigger path, args touched, and validation output.

Ask the operator before coding if any unforeseen problem changes the behavior contract.
