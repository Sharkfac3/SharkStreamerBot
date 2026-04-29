---
id: actions-temporary
type: domain-route
description: Temporary Streamer.bot timer bridge actions, ownership notes, paste targets, and Rest Focus relationship.
owner: streamerbot-dev
secondaryOwners: []
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Temporary — Agent Guide

## Purpose

This folder owns temporary Streamer.bot C# scripts that bridge a short-lived focus timer flow to Mix It Up. The current files start and end the `Temp Focus Timer` timer and trigger temporary Mix It Up action groups.

Ownership note: this route remains standalone in the manifest as `actions-temporary`. Operationally, the scripts are related to focus/rest experimentation, but they are not the same state machine as [Actions/Rest Focus Loop/](../Rest%20Focus%20Loop/) and should not be silently covered by that route without a later explicit migration.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Temporary/](./):

- [temp-focus-timer-start.cs](temp-focus-timer-start.cs) — triggers the temporary lock-in Mix It Up action and enables `Temp Focus Timer`.
- [temp-focus-timer-end.cs](temp-focus-timer-end.cs) — triggers the timer-end Mix It Up action when `Temp Focus Timer` completes.

This folder is for temporary operational bridges only. If a temporary script becomes permanent, either move its guidance into a durable route in a later scoped prompt or explicitly document why it remains here.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot timer wiring, Mix It Up API call shape, and paste readiness for this folder.

## Secondary Owners / Chain To

- `ops` — chain for Mix It Up command discovery, validation, sync, and final handoff workflow.
- `brand-steward` — chain only if public-facing timer wording, alert text, TTS, or viewer-facing copy changes.

## Required Reading

Before changing scripts, read:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for the Temporary timer constant.
- [Actions/Helpers/mixitup-command-api.md](../Helpers/mixitup-command-api.md) for the Streamer.bot C# Mix It Up API helper pattern.
- [Tools/MixItUp/AGENTS.md](../../Tools/MixItUp/AGENTS.md) for current Mix It Up API payload conventions and command-tooling guidance.
- [Actions/Rest Focus Loop/AGENTS.md](../Rest%20Focus%20Loop/AGENTS.md) only when evaluating whether temporary focus behavior should be retired or migrated into the durable rest/focus loop.
- [Tools/MixItUp/README.md](../../Tools/MixItUp/README.md) when discovering or refreshing Mix It Up command IDs.

## Local Workflow

1. Treat this folder as a temporary bridge, not a feature system with long-term state.
2. Preserve the `Temp Focus Timer` timer name unless the operator explicitly requests a rename.
3. Keep Mix It Up calls compatible with the shared convention: `Platform = Twitch`, `Arguments`, `SpecialIdentifiers`, and `IgnoreRequirements = false`.
4. Replace placeholder Mix It Up command IDs only with operator-confirmed command IDs.
5. Keep scripts self-contained and paste-ready for Streamer.bot inline C#.
6. If a script graduates into the durable rest/focus system, do not change manifest coverage in the same prompt unless explicitly requested. Flag the migration need in the handoff.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm the Streamer.bot timer named `Temp Focus Timer` exists.
- Confirm [temp-focus-timer-end.cs](temp-focus-timer-end.cs) is wired to the timer-end event.
- Trigger [temp-focus-timer-start.cs](temp-focus-timer-start.cs) and verify the timer starts.
- Let the timer complete and verify [temp-focus-timer-end.cs](temp-focus-timer-end.cs) runs.
- Verify Mix It Up command IDs are not placeholders before production use.

## Boundaries / Out of Scope

- Do not treat this folder as covered by [Actions/Rest Focus Loop/](../Rest%20Focus%20Loop/) during this prompt; manifest coverage remains separate.
- Do not add durable rest/focus state variables here.
- Do not rename or remove the temporary timer without explicit operator approval.
- Do not implement app-side or Mix It Up tooling changes here.

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

For code changes, list paste targets for both temporary scripts if either behavior changes. Include timer setup, command ID placeholders, and whether the operator should review retirement or migration into the Rest Focus Loop route.

## Runtime Notes

Current scripts:

| Script | Trigger | Side effect |
|---|---|---|
| [temp-focus-timer-start.cs](temp-focus-timer-start.cs) | Voice command or manual Streamer.bot action | Calls temporary lock-in Mix It Up command and enables `Temp Focus Timer`. |
| [temp-focus-timer-end.cs](temp-focus-timer-end.cs) | Timer end for `Temp Focus Timer` | Calls Captain Stretch lock-in timer-end Mix It Up command. |

Current ownership classification: standalone `actions-temporary` route, operationally adjacent to Rest Focus Loop but not a covered-by child route.
