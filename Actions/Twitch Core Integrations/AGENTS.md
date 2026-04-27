---
id: actions-twitch-core-integrations
type: domain-route
description: Core Twitch stream lifecycle, follow, subscription, watch-streak, reset, and Mix It Up bridge guidance.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Core Integrations — Agent Guide

## Purpose

This folder owns core Twitch event and stream-state Streamer.bot C# scripts: stream-start reset, new follows, subscriptions, gift subscriptions, sub counter rollover, and watch streaks. These scripts keep live stream state clean and forward Twitch event metadata to Mix It Up in a stable payload shape.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep core Twitch event knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Core Integrations/](./), including:

- [Actions/Twitch Core Integrations/stream-start.cs](stream-start.cs)
- [Actions/Twitch Core Integrations/follower-new.cs](follower-new.cs)
- [Actions/Twitch Core Integrations/subscription-new.cs](subscription-new.cs)
- [Actions/Twitch Core Integrations/subscription-renewed.cs](subscription-renewed.cs)
- [Actions/Twitch Core Integrations/subscription-prime-paid-upgrade.cs](subscription-prime-paid-upgrade.cs)
- [Actions/Twitch Core Integrations/subscription-gift-paid-upgrade.cs](subscription-gift-paid-upgrade.cs)
- [Actions/Twitch Core Integrations/subscription-pay-it-forward.cs](subscription-pay-it-forward.cs)
- [Actions/Twitch Core Integrations/subscription-gift.cs](subscription-gift.cs)
- [Actions/Twitch Core Integrations/subscription-counter-rollover.cs](subscription-counter-rollover.cs)
- [Actions/Twitch Core Integrations/watch-streak.cs](watch-streak.cs)
- README or operator documentation in this folder

Activate `brand-steward` before changing public follow/sub/watch-streak text, Mix It Up alert copy, overlay text, spoken/TTS text, or event announcement wording.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot trigger compatibility, stream-start reset behavior, session/global variable resets, Mix It Up API payload shape, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public event text, alert wording, overlay copy, TTS/spoken text, or subscription/follow/watch-streak announcement wording.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the local README before editing scripts:

- [Actions/Twitch Core Integrations/README.md](README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md)
- [Actions/Squad/AGENTS.md](../Squad/AGENTS.md) when changing stream-start resets for Squad or mini-game lock state
- [Actions/LotAT/AGENTS.md](../LotAT/AGENTS.md) when changing stream-start resets for LotAT-related state
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify whether the change is stream-start reset logic, a follow/subscription event bridge, watch-streak handling, or documentation-only.
2. Treat [stream-start.cs](stream-start.cs) as the central stream-session reset point. It must run early in Streamer.bot stream startup so downstream actions see clean state.
3. Any new global variable added by any Streamer.bot feature must be added to [stream-start.cs](stream-start.cs) reset logic and documented in [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md), unless the prompt explicitly excludes those files; if excluded, flag the required follow-up in the handoff.
4. Preserve existing trigger wiring and event-specific scripts. The five dedicated subscription scripts each map to a distinct Twitch subscription trigger.
5. Keep [subscription-gift.cs](subscription-gift.cs) as the smart dual-trigger handler for Gift Subscription and Gift Bomb. Preserve suppression of individual per-recipient gift events during gift bombs.
6. Preserve Mix It Up payload compatibility:
   - `Platform = "Twitch"`
   - `Arguments` remains empty where current commands expect empty arguments.
   - Structured event metadata belongs in populated `SpecialIdentifiers`.
   - Use lowercase, no-space special identifier keys.
7. If a command ID is a placeholder or missing, scripts should log a warning and exit safely rather than throwing.
8. Keep scripts self-contained and paste-ready. Do not assume shared repo helper files can be imported by Streamer.bot.
9. Update [Actions/Twitch Core Integrations/README.md](README.md) if trigger variables, reset state, payload identifiers, command IDs, operator wiring, or event routing changes.

Current script map:

| Script | Runtime purpose |
|---|---|
| [stream-start.cs](stream-start.cs) | Central stream-start reset for minigame locks, Squad state, LotAT-related state, OBS source visibility, timers, and `stream_mode` |
| [follower-new.cs](follower-new.cs) | New follow event bridge to Mix It Up |
| [subscription-new.cs](subscription-new.cs) | First-time subscription event bridge |
| [subscription-renewed.cs](subscription-renewed.cs) | Resubscription event bridge |
| [subscription-prime-paid-upgrade.cs](subscription-prime-paid-upgrade.cs) | Prime-to-paid upgrade event bridge for SB v0.2.5+ |
| [subscription-gift-paid-upgrade.cs](subscription-gift-paid-upgrade.cs) | Gift-paid upgrade event bridge for SB v0.2.5+ |
| [subscription-pay-it-forward.cs](subscription-pay-it-forward.cs) | Pay-it-forward event bridge for SB v0.2.5+ |
| [subscription-gift.cs](subscription-gift.cs) | Solo gift/gift bomb router, suppressing duplicate gift-bomb recipient fires |
| [subscription-counter-rollover.cs](subscription-counter-rollover.cs) | Sub counter rollover threshold event bridge |
| [watch-streak.cs](watch-streak.cs) | Twitch watch streak event bridge with populated watch-streak identifiers |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify all global names, OBS source names, timer names, and Mix It Up command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For [stream-start.cs](stream-start.cs), verify every new or changed feature state reset is documented and that reset values are safe for a fresh stream session.
- For Mix It Up payload changes, confirm `Arguments` compatibility and populated `SpecialIdentifiers` in [Actions/Twitch Core Integrations/README.md](README.md).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not change stream-start action ordering assumptions unless explicitly requested.
- Do not remove stream-start resets for Squad, LotAT-related variables, timers, or OBS source setup without validating the dependent feature guides.
- Do not rename global variables, timers, OBS sources, Streamer.bot action names, or special identifier keys unless explicitly requested and documented.
- Do not move bits, channel-point, hype-train, intro, Squad, or LotAT feature behavior into this guide.
- Do not add public event copy without `brand-steward` review.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Core Integrations/](./). Operator must manually paste changed script contents into the matching Streamer.bot Twitch Core Integrations actions and verify trigger wiring.

Public-copy handoff triggers: follow/sub/watch-streak alert text, overlay messages, TTS/spoken responses, event announcement copy, and Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.
