---
id: actions-rest-focus-loop
type: domain-route
description: Rest/focus loop Streamer.bot timer actions, commander integration points, setup, and validation notes.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Rest Focus Loop — Agent Guide

## Purpose

This folder owns the repeating rest/focus loop controller for Streamer.bot. The loop alternates pre-rest, rest, pre-focus, and focus phases using Streamer.bot timers, Mix It Up alerts, and commander override commands in related folders.

The loop is intentionally defensive: transition scripts set the next phase before arming timers, ignore stale timer fires, and fail closed by disabling timers and clearing active state if timer arming fails.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Rest Focus Loop/](./):

- [README.md](README.md) — detailed local script reference.
- [rest-focus-loop-start.cs](rest-focus-loop-start.cs) — starts or restarts the loop from pre-rest.
- [rest-focus-pre-rest-end.cs](rest-focus-pre-rest-end.cs) — pre-rest timeout into rest.
- [rest-focus-rest-end.cs](rest-focus-rest-end.cs) — rest timeout into pre-focus.
- [rest-focus-pre-focus-end.cs](rest-focus-pre-focus-end.cs) — pre-focus timeout into focus.
- [rest-focus-focus-end.cs](rest-focus-focus-end.cs) — focus timeout back to pre-rest.

Activate `brand-steward` before changing public rest/focus wording, Mix It Up alert text, stream-facing command descriptions, or any health/productivity framing that viewers will see or hear.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, timer names, Streamer.bot trigger wiring, global variable state, Mix It Up bridge behavior, and paste readiness.

## Secondary Owners / Chain To

- `brand-steward` — chain for public wording, alerts, TTS/scripted phrases, or viewer-facing rest/focus framing.
- `ops` — chain for validation, sync, and final handoff workflow.

Related action guides:

- [Actions/Commanders/AGENTS.md](../Commanders/AGENTS.md) for Captain Stretch and Water Wizard commands that can alter rest/focus timing.
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) for stream-start timer reset behavior.

## Required Reading

Before changing scripts, read:

- [Actions/Rest Focus Loop/README.md](README.md) for the current transition model and operator setup.
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for Rest / Focus Loop timer and global variable names.
- [Actions/Helpers/timers.md](../Helpers/timers.md) and [Actions/Helpers/cph-api-signatures.md](../Helpers/cph-api-signatures.md) for verified timer methods and helper patterns.
- [Actions/Commanders/Water Wizard/water-wizard-castrest.cs](../Commanders/Water%20Wizard/water-wizard-castrest.cs) when changing rest override behavior.
- [Actions/Commanders/Captain Stretch/captain-stretch-generalfocus.cs](../Commanders/Captain%20Stretch/captain-stretch-generalfocus.cs) when changing focus override behavior.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

1. Identify which phase transition is affected: start, pre-rest end, rest end, pre-focus end, or focus end.
2. Preserve the phase guard model. Timer-end scripts should only advance when `rest_focus_loop_active` is true and `rest_focus_loop_phase` matches the expected phase.
3. Preserve the fail-closed behavior when timers cannot be armed.
4. Keep timer names exactly aligned with [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
5. Treat `CPH.SetTimerInterval(string, int)` as a verified-in-production dependency only after the operator confirms the current Streamer.bot build supports it. The helper docs still mark it for verification.
6. When changing durations or timer behavior, review the commander override scripts as well as the five loop scripts.
7. Update [README.md](README.md) whenever trigger wiring, phases, timers, Mix It Up command IDs, durations, or recovery steps change.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-06-validator.failures.txt
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm the four Streamer.bot timers exist with exact names from [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Confirm each timer-end event triggers the matching script.
- Test loop start and each phase transition in a controlled Streamer.bot session.
- Test stale timer behavior by firing a timer-end action in the wrong phase and confirming it logs and exits.
- Verify configured Mix It Up command IDs before production use.
- Confirm stream-start disables all four loop timers and clears loop state.

## Boundaries / Out of Scope

- Do not move commander override command behavior into this folder; keep it in [Actions/Commanders/](../Commanders/).
- Do not rename timer names, global variables, phases, or Streamer.bot action names without explicit operator approval.
- Do not change rest/focus public messaging without `brand-steward` review.
- Do not merge [Actions/Temporary/](../Temporary/) into this route during this coverage-fill prompt; keep manifest coverage separate unless a later prompt explicitly changes route ownership.

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

For code changes, list paste targets for every edited script and include timer wiring, placeholder Mix It Up IDs, `CPH.SetTimerInterval` verification status, and any commander override follow-up.

## Runtime Notes

Required Streamer.bot timers:

| Timer | Expected action |
|---|---|
| `Rest Focus - Pre Rest` | [rest-focus-pre-rest-end.cs](rest-focus-pre-rest-end.cs) |
| `Rest Focus - Rest` | [rest-focus-rest-end.cs](rest-focus-rest-end.cs) |
| `Rest Focus - Pre Focus` | [rest-focus-pre-focus-end.cs](rest-focus-pre-focus-end.cs) |
| `Rest Focus - Focus` | [rest-focus-focus-end.cs](rest-focus-focus-end.cs) |

Core state variables:

| Variable | Meaning |
|---|---|
| `rest_focus_loop_active` | True while the loop owns rest/focus timing. |
| `rest_focus_loop_phase` | Current phase: `idle`, `pre_rest`, `rest`, `pre_focus`, or `focus`. |

The local [README.md](README.md) remains the detailed script-by-script reference for this route.
