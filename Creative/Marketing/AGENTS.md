---
id: creative-marketing
type: domain-route
description: Marketing, community growth, public copy scaffolding, clip strategy, and platform packaging handoffs.
owner: brand-steward
secondaryOwners:
  - content-repurposer
workflows:
  - canon-guardian
  - change-summary
status: active
---

# Creative/Marketing — Agent Guide

## Purpose

[Creative/Marketing/](./) contains campaign scaffolding, promotional copy, social content strategy, community growth planning, and public-facing content systems for the SharkFac3 and Starship Shamples brand.

Use this folder for the bridge between brand strategy and distributed content: stream title frameworks, community posts, clip strategy, campaign plans, social formatting guidance, and prompts for marketing agents. This guide migrates the former community-growth, content-strategy, clip-strategy, and platforms guidance into one local route with `brand-steward` primary ownership and `content-repurposer` as the platform-packaging specialist.

## When to Activate

Use this guide when working on:

- stream titles, descriptions, community announcements, and campaign copy
- community growth strategy for Twitch, Discord, and social discovery
- clip strategy and highlight planning
- short-form platform packaging for YouTube Shorts, TikTok, Instagram Reels, or related social formats
- social post frameworks under [Creative/Marketing/Social/](Social/)
- campaign plans under [Creative/Marketing/Campaigns/](Campaigns/)
- reusable copy templates under [Creative/Marketing/Copy/](Copy/)
- marketing agent prompts under [Creative/Marketing/Agents/](Agents/)

Do not activate this guide for raw content-pipeline tooling; use [Tools/ContentPipeline/AGENTS.md](../../Tools/ContentPipeline/AGENTS.md) for transcription, highlight detection, extraction, review queues, and publishing data mechanics.

## Primary Owner

Primary owner: `brand-steward`.

`brand-steward` owns the public-facing strategy, brand voice, community tone, campaign intent, and whether growth work respects the brand pillars.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `content-repurposer` | Clip selection, captions, platform formatting, content calendars, or short-form packaging is needed. |
| `lotat-writer` | A campaign, clip plan, or stream hook depends on LotAT story themes or adventure planning. |
| `streamerbot-dev` | A growth idea requires Twitch chat actions, redemption text, mini-games, or stream automation. |
| `app-dev` | A growth idea requires overlays, dashboards, production UI, or app-side interaction changes. |
| `product-dev` | Marketing work becomes product-facing documentation, knowledge articles, or launch collateral. |
| `ops` | Validation, agent-tree maintenance, or change-summary formatting is needed. |

Run the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) if marketing work introduces new recurring lore, changes character interpretation, implies permanent canon, or packages a joke in a way that could become franchise truth.

## Required Reading

Read these first for marketing work:

1. [Creative/Marketing/README.md](README.md) — folder scope and brand voice reminders.
2. [Creative/Brand/BRAND-IDENTITY.md](../Brand/BRAND-IDENTITY.md) — brand foundation, business model, values, and content pipeline.
3. [Creative/Brand/BRAND-VOICE.md](../Brand/BRAND-VOICE.md) — copy tone by context.
4. [Creative/Brand/CHARACTER-CODEX.md](../Brand/CHARACTER-CODEX.md) when characters or Starship Shamples jokes are used.
5. [Tools/ContentPipeline/AGENTS.md](../../Tools/ContentPipeline/AGENTS.md) when strategy turns into pipeline tooling or review-queue work.
6. [Creative/WorldBuilding/AGENTS.md](../WorldBuilding/AGENTS.md) when a campaign depends on LotAT adventure or franchise continuity.

## Local Workflow

1. Start from the real stream context. Marketing should be specific to the actual build, problem, stream moment, story arc, or community event.
2. Keep the community-as-crew framing. Growth should not make regulars feel like passive consumers or public-facing conversion targets.
3. Use the build-as-mission bridge when useful. A real build problem can map to a Starship Shamples mission beat: Pedro makes fixes worse, Duck provides terrible ideas, Clone appears without context, Toothless responds too hard, or Captain Stretch tries to regain control.
4. Select clip opportunities by peak value:
   - high chat activity, laughter, surprise, frustration, or visual drama
   - technical insight, entertainment payoff, or both
   - enough standalone value for someone who has never seen the stream
   - natural short-form start and end points, usually around 30 to 60 seconds
5. Package for platform:
   - YouTube Shorts: technical, searchable, concise title, strong educational angle.
   - TikTok: chaos, entertainment, strong first two seconds, conversational caption.
   - Instagram Reels: polished visual moments, community building, carousel support for step-by-step technical content.
6. Avoid growth-hack public language. Internally, strategy may talk about discovery and retention; externally, the brand should sound like a real community sharing work worth watching.
7. Chain to `content-repurposer` when a clip needs captions, platform-specific formats, calendars, or review/publishing workflow.

## Validation

For marketing-doc or local agent-guide changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

For public marketing output, validate by checklist:

- The hook is specific and honest, not manufactured hype.
- The output is warm, accessible, and community-forward.
- Failure and chaos are framed as content, not shame.
- Technical claims are accurate and understandable.
- Platform constraints are considered before final copy.
- Any character or lore reference matches [Creative/Brand/CHARACTER-CODEX.md](../Brand/CHARACTER-CODEX.md).

For pipeline implementation or tooling validation, use [Tools/ContentPipeline/AGENTS.md](../../Tools/ContentPipeline/AGENTS.md).

## Boundaries / Out of Scope

Do not use this folder to:

- implement transcription, clip extraction, or publishing tooling
- invent new brand pillars or change the brand metaphor without [Creative/Brand/AGENTS.md](../Brand/AGENTS.md)
- write full LotAT story JSON without [Creative/WorldBuilding/AGENTS.md](../WorldBuilding/AGENTS.md)
- generate art assets without [Creative/Art/AGENTS.md](../Art/AGENTS.md)
- use clickbait, shame, artificial urgency, or generic hype as a growth strategy
- make the audience feel optimized, monetized, or treated as a funnel in public-facing copy

## Handoff Notes

Marketing handoffs should include:

- target channel or platform
- content type: title, announcement, clip plan, campaign, caption, calendar, or strategy note
- brand docs read and voice constraints applied
- clip category when relevant: technical, entertainment, or both
- platform constraints considered
- chain-to status for `content-repurposer`, `lotat-writer`, `streamerbot-dev`, or `product-dev`
- canon classification and [canon-guardian](../../.agents/workflows/canon-guardian.md) status when lore or character interpretation appears

Canon-sensitive marketing that needs operator review includes campaign language that introduces new recurring lore, permanently reframes a character, changes the Starship Shamples metaphor, or presents a one-off adventure joke as durable franchise canon.
