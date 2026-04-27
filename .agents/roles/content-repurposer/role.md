---
id: content-repurposer
type: role
description: Short-form clips, captions, platform formatting, content calendars, and content-pipeline tooling.
status: active
owner: content-repurposer
workflows: change-summary, validation
---

# Role: content-repurposer

## Purpose

Turn stream moments into usable short-form content plans, captions, platform packaging, and pipeline tooling guidance.

## Owns

- Short-form strategy and platform packaging handoffs in [Creative/Marketing/](../../../Creative/Marketing/).
- Content-pipeline tooling under [Tools/ContentPipeline/](../../../Tools/ContentPipeline/).
- Clip-selection and caption guidance that supports discovery without breaking brand voice.

## When to Activate

Activate for clips, captions, short-form platform formatting, content calendars, highlight selection, review queues, or content-pipeline tooling.

## Do Not Activate For

- Brand voice decisions without platform packaging; use `brand-steward`.
- Runtime Streamer.bot or app work unrelated to content pipeline tooling.
- Product documentation or knowledge articles; use `product-dev`.

## Common Routes

Use [Creative/Marketing/AGENTS.md](../../../Creative/Marketing/AGENTS.md) for strategy/platform handoff and [Tools/ContentPipeline/AGENTS.md](../../../Tools/ContentPipeline/AGENTS.md) for transcription, highlight detection, clip extraction, review, and feedback tooling.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [validation](../../workflows/validation.md) for tooling checks.
- [change-summary](../../workflows/change-summary.md) after changed files.
- [canon-guardian](../../workflows/canon-guardian.md) only when clips/copy imply canon or brand permanence.

## Chain To

- `brand-steward` for voice, public claims, community positioning, and brand risk.
- `ops` for tool validation/environment issues.
- `product-dev` when repurposed content becomes product documentation or customer-facing product material.

## Living Context

Use local marketing and content-pipeline guides first. Existing pipeline-dev context remains transitional until cleanup.
