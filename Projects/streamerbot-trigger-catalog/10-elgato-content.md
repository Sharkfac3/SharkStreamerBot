---
id: trigger-catalog-phase-10-elgato-content
type: project-phase
description: Fill every Elgato subcategory catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 10
depends_on:
  - 02-wiring.md
---

# Phase 10 — Elgato Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **Elgato** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/elgato/*.md` (skeleton).
- Manifest: Elgato platform entries.
- Used-in-repo backfill: repo does not currently use Elgato hardware integrations directly via Streamer.bot triggers (Stream Deck integration may exist via separate config, not Streamer.bot triggers). Expect `_Not yet wired._`.

## Elgato subcategories (from manifest)

Camera Hub, Stream Deck, Wave Link.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/elgato/`.

## Exit

Dirty tree.

## Next phase

Independent — run any remaining content phase.
