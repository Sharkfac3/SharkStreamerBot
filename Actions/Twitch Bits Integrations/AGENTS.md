---
id: actions-twitch-bits-integrations
type: domain-route
description: Twitch bits and automatic reward Streamer.bot event bridges, Mix It Up payloads, pacing, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Bits Integrations — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch cheer/bits tiers and related automatic reward integrations that forward event data to Mix It Up. These scripts are bridge actions: they sanitize viewer-provided text, preserve the current Mix It Up payload contract, and keep audio/readout timing safe during live stream events.

This is part of the ratified Twitch target shape: one `streamerbot-dev` owner family with folder-local Twitch guides. Do not recreate flat Twitch wrapper skills; keep folder-specific runtime knowledge here.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Twitch Bits Integrations/](./), including:

- [Actions/Twitch Bits Integrations/bits-tier-1.cs](bits-tier-1.cs)
- [Actions/Twitch Bits Integrations/bits-tier-2.cs](bits-tier-2.cs)
- [Actions/Twitch Bits Integrations/bits-tier-3.cs](bits-tier-3.cs)
- [Actions/Twitch Bits Integrations/bits-tier-4.cs](bits-tier-4.cs)
- [Actions/Twitch Bits Integrations/gigantify-emote.cs](gigantify-emote.cs)
- [Actions/Twitch Bits Integrations/message-effects.cs](message-effects.cs)
- [Actions/Twitch Bits Integrations/on-screen-celebration.cs](on-screen-celebration.cs)
- README or operator documentation in this folder

Activate `brand-steward` before changing public cheer/readout text, automatic reward wording, Mix It Up message copy, reward prompts, or any other viewer-facing event text.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot trigger compatibility, Mix It Up API payload shape, timing/wait behavior, and manual paste readiness for this folder.

## Secondary Owners / Chain To

- `brand-steward` — chain for public event/readout text, reward names/descriptions, overlay copy, or any text that viewers hear or see.
- `ops` — chain only for validation, paste/sync workflow, or agent-tree maintenance beyond this local guide.

## Required Reading

Read the local README before editing scripts:

- [Actions/Twitch Bits Integrations/README.md](README.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md)
- [Actions/Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes affect stream-start reset or shared Twitch event conventions
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes

## Local Workflow

1. Identify whether the change is a cheer tier script, an automatic reward script, or documentation-only.
2. Preserve the Streamer.bot trigger contract. Cheer-tier scripts are triggered by Twitch cheer events; automatic reward scripts are wired to `Twitch -> Channel Reward -> Automatic Reward Redemption` and filtered to the intended reward.
3. Keep Mix It Up payloads compatible:
   - `Platform = "Twitch"`
   - `Arguments` preserves the existing command behavior.
   - Structured metadata belongs in populated `SpecialIdentifiers` with lowercase, no-space keys.
   - `IgnoreRequirements = false` unless the operator explicitly requests otherwise.
4. Read Streamer.bot args defensively with safe fallbacks. Prefer `messageStripped` for cheer text, then `message`, then `rawInput`.
5. Preserve cheer token sanitization and word caps:
   - [bits-tier-1.cs](bits-tier-1.cs) — no word cap
   - [bits-tier-2.cs](bits-tier-2.cs) — 250-word cap
   - [bits-tier-3.cs](bits-tier-3.cs) — 100-word cap
   - [bits-tier-4.cs](bits-tier-4.cs) — 10-word cap
6. Preserve dynamic wait behavior for readout-style scripts: `3000ms + 400ms per word + 500ms buffer` where currently used.
7. Keep scripts lightweight and self-contained. Do not assume shared repo helper files can be imported by Streamer.bot.
8. Update [Actions/Twitch Bits Integrations/README.md](README.md) if trigger variables, payload identifiers, command IDs, waits, or operator wiring changes.
9. If a new global variable, OBS source, timer, or shared command contract is introduced, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope or flag it in the handoff when out of scope.

Current script map:

| Script | Runtime purpose |
|---|---|
| [bits-tier-1.cs](bits-tier-1.cs) | Tier 1 cheer text to Mix It Up, no word cap, paced readout wait |
| [bits-tier-2.cs](bits-tier-2.cs) | Tier 2 cheer text to Mix It Up, 250-word cap, paced readout wait |
| [bits-tier-3.cs](bits-tier-3.cs) | Tier 3 cheer text to Mix It Up, 100-word cap, paced readout wait |
| [bits-tier-4.cs](bits-tier-4.cs) | Tier 4 cheer text to Mix It Up, 10-word cap, paced readout wait |
| [gigantify-emote.cs](gigantify-emote.cs) | Automatic reward bridge for gigantified emote metadata |
| [message-effects.cs](message-effects.cs) | Automatic reward bridge for viewer-entered message effects text |
| [on-screen-celebration.cs](on-screen-celebration.cs) | Automatic reward bridge for on-screen celebration metadata |

## Validation

For script changes, perform the narrowest safe validation available:

- Review edited C# for Streamer.bot paste readiness: one `Execute()` entry point, no unsupported imports, no repo-only runtime dependencies, and defensive arg handling.
- Verify global names, OBS names, timers, bit tier constants, and command contracts against [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- For Mix It Up payload changes, confirm `Arguments` compatibility and populated `SpecialIdentifiers` in [Actions/Twitch Bits Integrations/README.md](README.md).
- Run shared-constants validation when constants or documented references change:

```bash
python3 Tools/StreamerBot/validate-shared-constants.py
```

For agent-doc changes, follow [validation](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path. Record command output in the handoff or final change summary.

## Boundaries / Out of Scope

- Do not change cheer tier thresholds or reward wiring unless explicitly requested.
- Do not hardcode bit threshold values in scripts; use [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) as the canonical reference.
- Do not rename Mix It Up special identifier keys without updating the matching Mix It Up commands and README payload contract.
- Do not move channel-point redemption scripts from [Actions/Twitch Channel Points/](../Twitch%20Channel%20Points/) into this folder unless the operator explicitly requests a repo reorganization.
- Do not move non-Bits Twitch docs into this guide unless the operator explicitly requests a repo reorganization.

## Handoff Notes

After changes, follow these workflows:

- [change-summary](../../.agents/workflows/change-summary.md) — terminal summary with changed files, paste targets, setup steps, and validation output.
- [sync](../../.agents/workflows/sync.md) — repo-to-Streamer.bot manual paste expectations.
- [validation](../../.agents/workflows/validation.md) — validation command selection and failure reporting.

Paste targets are the edited `.cs` files under [Actions/Twitch Bits Integrations/](./). Operator must manually paste changed script contents into the matching Streamer.bot actions and verify cheer-tier or automatic-reward trigger filtering.

Public-copy handoff triggers: cheer/readout wording, automatic reward prompts, overlay message text, TTS text, or Mix It Up text branches. Include exactly which strings changed and whether `brand-steward` reviewed them.
