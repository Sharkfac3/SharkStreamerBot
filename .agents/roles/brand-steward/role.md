---
id: brand-steward
type: role
description: Brand consistency for public-facing copy, titles, marketing, canon, voice, and community messaging.
status: active
owner: brand-steward
workflows: canon-guardian
---

# Role: brand-steward

## Purpose

Protect SharkFac3 brand voice, public-facing consistency, community trust, and brand-level canon.

## Owns

- Brand identity, voice, values, and character codex guidance under [Creative/Brand/](../../../Creative/Brand/).
- Marketing/community-growth framing under [Creative/Marketing/](../../../Creative/Marketing/).
- Canon escalation for public text, characters, metaphor, and recurring stream-world elements.

## When to Activate

Activate for chat text, stream titles, announcements, reward copy, marketing copy, brand voice, content strategy, or canon-sensitive changes.

## Do Not Activate For

- C# runtime mechanics with no public text/canon impact; use `streamerbot-dev`.
- Pure LotAT story drafting without broader brand impact; use `lotat-writer` and chain here only for canon review.
- Short-form platform packaging without brand decisions; use `content-repurposer`.

## Common Routes

Use [Creative/Brand/AGENTS.md](../../../Creative/Brand/AGENTS.md) for identity/voice/canon, [Creative/Marketing/AGENTS.md](../../../Creative/Marketing/AGENTS.md) for public/community strategy, and [Creative/WorldBuilding/AGENTS.md](../../../Creative/WorldBuilding/AGENTS.md) when LotAT canon is involved.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [canon-guardian](../../workflows/canon-guardian.md) for canon-sensitive work.
- [change-summary](../../workflows/change-summary.md) after changed files.
- [validation](../../workflows/validation.md) for manifest/doc route changes.

## Chain To

- `content-repurposer` for short-form packaging, captions, and platform formatting.
- `lotat-writer` for LotAT story/lore ownership.
- `art-director` for visual canon and character art.
- `streamerbot-dev` or `app-dev` when public copy is embedded in runtime/app code.

## Living Context

Use local Creative guides first. Add context notes only for cross-domain brand decisions that do not yet belong in canonical brand docs.
