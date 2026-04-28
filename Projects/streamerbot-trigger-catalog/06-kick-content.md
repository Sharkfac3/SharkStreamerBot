---
id: trigger-catalog-phase-06-kick-content
type: project-phase
description: Fill every Kick subcategory catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 6
depends_on:
  - 02-wiring.md
---

# Phase 6 — Kick Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **Kick** trigger.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/kick/*.md` (skeleton).
- Manifest: Kick platform entries.
- Used-in-repo backfill scope: this repo currently does not stream on Kick. Expect every `Used in repo` to read `_Not yet wired._`. Document anyway — future agents may wire Kick alongside Twitch.

## Kick subcategories (from manifest)

Channel, Channel Reward, Chat, Emotes, General, Kicks, Moderation, Subscriptions.

## Kick-specific notes

- Many Kick triggers parallel Twitch counterparts (Sub, Gift, Cheer-equivalents). Where args names differ, document them fresh — do not paraphrase from Twitch entries.
- Kick has a unique `Kicks` subcategory (named after the `Kick` site's tip mechanism). Confirm trigger args by fetching the upstream pages; do not assume.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/kick/`.

## Exit

Dirty tree. Change summary lists Kick trigger count.

## Next phase

Independent — run any of `07`–`13`, `14` (after `05`), or stop and commit.
