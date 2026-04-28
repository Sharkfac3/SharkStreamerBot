---
id: trigger-catalog-phase-09-integrations-content
type: project-phase
description: Fill every Integrations subcategory (one per third-party service) catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 9
depends_on:
  - 02-wiring.md
---

# Phase 9 — Integrations Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **Integrations** trigger across all 19 third-party services.

This is the largest content phase. Self-split into sub-passes if needed — one service per sub-pass is reasonable.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/integrations/*.md` (skeleton — one file per service).
- Manifest: Integrations platform entries.
- Used-in-repo backfill: this repo does not currently use any of these integrations. Expect every `Used in repo` to read `_Not yet wired._`. Document anyway.

## Integrations services (one catalog file per service, from manifest)

Crowdcontrol, Donordrive, Fourthwall, HypeRate.io, Ko-Fi, Pally.gg, Patreon, Pulsoid, Shopify, Speaker.bot, StreamElements, Streamer.bot, Streamerbot Remote, Streamlabs, Tipeeestream, T.I.T.S., Treatstream, Voicemod, Vtube Studio.

## Sub-pass guidance

Recommended split if the session gets long:

1. Donations / monetization: Donordrive, Ko-Fi, Patreon, Pally.gg, Streamlabs, StreamElements, Tipeeestream.
2. Hardware / streaming peripherals: HypeRate.io, Pulsoid, T.I.T.S., Voicemod, Vtube Studio.
3. Game integrations: Crowdcontrol, Treatstream.
4. Commerce: Fourthwall, Shopify.
5. Streamer.bot self-integrations: Speaker.bot, Streamer.bot, Streamerbot Remote.

Each sub-pass = one or more services from the same group. Track progress in `Projects/streamerbot-trigger-catalog/manifest.json` (or in commit messages — but per project rules do not commit; use change summary instead).

## Notes

- Many integration services have near-identical "donation received" or "tip received" triggers. Document each fresh — args names differ between providers.
- Some services may have no triggers documented upstream (only actions). Note explicitly: `coverage: seeded`, body says `_No triggers exposed by this integration._`.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/integrations/`.

## Exit

Dirty tree.

## Next phase

Independent — run any remaining content phase.
