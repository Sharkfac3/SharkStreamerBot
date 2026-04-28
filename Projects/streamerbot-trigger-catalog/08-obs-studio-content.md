---
id: trigger-catalog-phase-08-obs-studio-content
type: project-phase
description: Fill every OBS Studio subcategory catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 8
depends_on:
  - 02-wiring.md
---

# Phase 8 — OBS Studio Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **OBS Studio** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/obs-studio/*.md` (skeleton).
- Manifest: OBS Studio platform entries.
- Used-in-repo backfill scope: any `.cs` interacting with OBS scenes/sources, especially under `Actions/Voice Commands/` (scene-* scripts), `Actions/Twitch Channel Points/disco-party.cs`, `Actions/Squad/*/overlay-publish.cs`, and `Actions/Helpers/obs-scenes.md` cross-references.

## OBS Studio subcategories (from manifest)

Uncategorized (per upstream nav). All OBS triggers live in this single subcategory upstream — the catalog file is `Actions/Helpers/triggers/obs-studio/uncategorized.md`.

## OBS-specific notes

- OBS triggers cover scene/source/recording/streaming events. Several scripts in this repo *send* OBS commands but few *react* to OBS events — backfill carefully, many will be `_Not yet wired._`.
- Cross-reference `Actions/Helpers/obs-scenes.md` for the OBS interaction patterns the repo uses; some of those patterns may correspond to triggers worth documenting.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/obs-studio/`.

## Exit

Dirty tree.

## Next phase

Independent — run any remaining content phase.
