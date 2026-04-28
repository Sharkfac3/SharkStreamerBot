---
id: trigger-catalog-phase-12-streamlabs-desktop-content
type: project-phase
description: Fill the Streamlabs Desktop catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 12
depends_on:
  - 02-wiring.md
---

# Phase 12 — Streamlabs Desktop Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **Streamlabs Desktop** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/streamlabs-desktop/*.md` (skeleton).
- Manifest: Streamlabs Desktop platform entries.
- Used-in-repo backfill: repo does not currently use Streamlabs Desktop. Expect `_Not yet wired._`.

## Streamlabs Desktop subcategories (from manifest)

Uncategorized.

## Note

Do not confuse with the **Streamlabs** integration under Phase 9 (web service for tips/alerts). This phase covers Streamlabs Desktop, the OBS-fork streaming software.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/streamlabs-desktop/`.

## Exit

Dirty tree.

## Next phase

Independent — run any remaining content phase.
