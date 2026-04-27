---
id: actions-twitch-channel-points
type: domain-route
description: Twitch channel point redemption Streamer.bot actions, OBS/Mix It Up behavior, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Channel Points — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch channel point redemptions that are maintained in the repo. Current actions cover Disco Party and Explain Current Task. These scripts bridge Twitch redemptions into OBS scene changes, recording checks, and Mix It Up commands.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep channel-point redemption knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Channel Points/](./), including:

- [Actions/Twitch Channel Points/disco-party.cs](disco-party.cs)
- [Actions/Twitch Channel Points/explain-current-task.cs](explain-current-task.cs)
- README or operator documentation in this folder

Activate `brand-steward` before changing public redemption copy, reward names/descriptions, chat messages, overlay text, spoken/TTS text, or any viewer-facing Mix It Up response text.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot reward wiring expectations, OBS interactions, Mix It Up API payload shape, global variable use, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public redemption text, reward prompts, chat responses, overlay copy, or Mix It Up response wording.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the local README before editing scripts:

- [Actions/Twitch Channel Points/README.md](README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md)
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes add/reset shared session state
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify the redemption and its Streamer.bot action wiring.
2. Preserve existing reward names, reward IDs, command IDs, and trigger wiring unless the operator explicitly requests a migration.
3. Read Twitch reward args defensively with `CPH.TryGetArg`. Text-input rewards may expose viewer text through `userInput`, `input0`, `message`, or `rawInput` depending on trigger wiring/version.
4. Preserve `stream_mode` behavior for OBS scene routing. Unknown or missing mode should fall back safely to `workspace` behavior.
5. Use direct OBS methods from [Actions/Helpers/obs-scenes.md](../Helpers/obs-scenes.md). Do not use reflection to discover OBS methods.
6. Preserve Mix It Up payload compatibility:
   - Keep `Arguments` behavior compatible with the current Mix It Up command.
   - Put structured redemption metadata in populated `SpecialIdentifiers`.
   - Use lowercase, no-space special identifier keys.
7. Keep scripts self-contained and paste-ready. Do not assume shared repo helper files can be imported by Streamer.bot.
8. Update [Actions/Twitch Channel Points/README.md](README.md) if trigger variables, payload identifiers, command IDs, OBS behavior, wait behavior, or operator wiring changes.
9. If a new global variable, OBS source, timer, or shared command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

Current script map:

| Script | Runtime purpose | Important notes |
|---|---|---|
| [disco-party.cs](disco-party.cs) | Runs Disco Party start/end Mix It Up commands, switches to mode-matched Disco Party OBS scene, fires unlocked squad dance commands, then returns to the previous scene | Uses `stream_mode`, `disco_party_active`, `disco_party_prev_scene`, unlock flags, and exact OBS scene names |
| [explain-current-task.cs](explain-current-task.cs) | Ensures OBS recording is active, then triggers the Explain Current Task Mix It Up flow | Current Mix It Up command ID is a placeholder in the README and script contract |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify global names, OBS scenes, timers, and Mix It Up command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For OBS changes, verify the exact scene names documented in [Actions/Twitch Channel Points/README.md](README.md): `Disco Party: Garage`, `Disco Party: Workspace`, and `Disco Party: Gamer`.
- For Mix It Up payload changes, confirm `Arguments` compatibility and populated `SpecialIdentifiers` in [Actions/Twitch Channel Points/README.md](README.md).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not rename channel point rewards, reward IDs, OBS scenes, or Mix It Up identifier keys unless explicitly requested.
- Do not add public redemption copy without `brand-steward` review.
- Do not move automatic rewards from [Actions/Twitch Bits Integrations/](../Twitch%20Bits%20Integrations/) into this folder unless the operator explicitly requests a repo reorganization.
- Custom intro redemptions are handled by [Actions/Intros/](../Intros/) and are not owned here.
- `Explain: Ask Away` is a known gap in Streamer.bot but has no matching repo script in this folder yet; do not invent the script unless requested.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Channel Points/](./). Operator must manually paste changed script contents into the matching Streamer.bot channel-point actions and verify reward trigger wiring.

Public-copy handoff triggers: reward names/descriptions, redemption prompts, chat messages, overlay messages, TTS/spoken responses, and Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.
