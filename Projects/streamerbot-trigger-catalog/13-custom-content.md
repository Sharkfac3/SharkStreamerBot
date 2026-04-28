---
id: trigger-catalog-phase-13-custom-content
type: project-phase
description: Fill the Custom triggers catalog file with args, version notes, caveats, used-in-repo backlinks.
status: active
owner: streamerbot-dev
phase: 13
depends_on:
  - 02-wiring.md
---

# Phase 13 — Custom Triggers Content Fill

## Goal

Apply the Phase 3 content-fill procedure to every **Custom** trigger documented upstream.

## Procedure

Use the procedure from [03-twitch-content.md](03-twitch-content.md) verbatim. Differences:

- Inputs: `Actions/Helpers/triggers/custom/*.md` (skeleton).
- Manifest: Custom platform entries.
- Used-in-repo backfill scope: any `.cs` that defines or fires a custom Streamer.bot trigger via `CPH.TriggerCodeEvent` or similar; check `Actions/Overlay/`, `Actions/LotAT/`, and any cross-script orchestration patterns.

## Custom subcategories (from manifest)

Uncategorized.

## Custom-specific notes

- Custom triggers are user-defined — upstream documents the *mechanism* (how to register, fire, handle) rather than a fixed args set. The catalog entry should describe:
  - How a custom trigger is defined in Streamer.bot.
  - The args contract (whatever the firing code chose to pass).
  - Patterns this repo uses (if any) — e.g. `actionName` conventions, `eventSource` keys.
- If upstream lists no fixed triggers, the file body should describe the mechanism + repo conventions instead of a per-trigger heading list.

## Validation

Per Phase 3 validation checklist, scoped to `Actions/Helpers/triggers/custom/`. Note: validation step 3 (file count vs manifest) may not apply if the file is mechanism-documentation rather than per-trigger headings — record that exception in the change summary.

## Exit

Dirty tree.

## Next phase

After this phase plus all earlier content phases complete, run `14-script-schema-fanout.md` (depends on Phase 5 pilot being approved by operator).
