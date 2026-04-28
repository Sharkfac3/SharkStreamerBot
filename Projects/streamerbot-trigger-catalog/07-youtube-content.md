---
id: trigger-catalog-phase-07-youtube-content
type: project-phase
description: Fill every YouTube subcategory catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 7
depends_on:
  - 02-wiring.md
---

# Phase 7 — YouTube Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **YouTube** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/youtube/*.md` (skeleton).
- Manifest: YouTube platform entries.
- Used-in-repo backfill: repo does not currently stream on YouTube. Expect `_Not yet wired._` everywhere.

## YouTube subcategories (from manifest)

Broadcast, Chat, General, Membership, Polls.

## YouTube-specific notes

- YouTube `Membership` parallels Twitch Subscriptions but with different args. Document fully without leaning on Twitch knowledge.
- YouTube `Broadcast` exposes stream lifecycle events (start, end). Catalog these as the YouTube equivalent of Twitch's `Stream Online` / `Stream Offline`.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/youtube/`.

## Exit

Dirty tree.

## Next phase

Independent — run any remaining content phase.
