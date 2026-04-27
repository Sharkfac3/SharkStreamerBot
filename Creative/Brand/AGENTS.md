---
id: creative-brand
type: domain-route
description: Brand identity, voice, character codex, metaphor, and canon source documents for public-facing work.
owner: brand-steward
secondaryOwners:
  - lotat-writer
  - art-director
workflows:
  - canon-guardian
status: active
---

# Creative/Brand — Agent Guide

## Purpose

[Creative/Brand/](./) is the source of truth for SharkStreamerBot and Starship Shamples brand identity, voice, values, character identity, and the neurodivergent metaphor that connects the live R&D build to the stream entertainment layer.

Use this folder whenever public-facing text, canon interpretation, character identity, community tone, or franchise meaning is at stake. This guide migrates the former brand voice routing into a local brand entrypoint while leaving marketing/community growth execution in [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md).

## When to Activate

Use this guide when working on:

- brand identity, values, mission, or positioning
- brand voice and tone by context
- chat bot response style, stream titles, community announcements, or public copy conventions
- character identity, metaphor roles, and canonical cast behavior
- canon review triggers for character, story, art, or marketing changes
- source docs under [Creative/Brand/](./)

Do not activate this guide for platform packaging or clip distribution by itself; use [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md) and chain to `content-repurposer` there.

## Primary Owner

Primary owner: `brand-steward`.

`brand-steward` owns public voice, brand consistency, values, metaphor fit, character identity interpretation, and approval boundaries for canon-sensitive brand decisions.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `lotat-writer` | Brand guidance affects LotAT adventures, lore, franchise continuity, or story narration. |
| `art-director` | Brand or character identity affects visual canon, art prompts, or approved stream assets. |
| `content-repurposer` | Brand voice needs to be adapted for clips, captions, social posts, or content calendars. |
| `streamerbot-dev` | Brand copy is embedded in Twitch chat actions, command responses, or automated event text. |
| `ops` | Agent-tree validation, handoff formatting, or repository workflow support is needed. |

Run the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) whenever a change could alter character identity, metaphor mapping, permanent world elements, recurring lore, or the way Starship Shamples canon is understood beyond one disposable joke.

## Required Reading

Read these source documents before brand work:

1. [Creative/Brand/BRAND-IDENTITY.md](BRAND-IDENTITY.md) — foundational why: business context, values, neurodivergent metaphor, and content pipeline.
2. [Creative/Brand/BRAND-VOICE.md](BRAND-VOICE.md) — tone and language guidance for narration, chat, titles, community posts, and social clips.
3. [Creative/Brand/CHARACTER-CODEX.md](CHARACTER-CODEX.md) — canonical character identities, appearances, behaviors, and metaphor roles.
4. [Creative/README.md](../README.md) — creative-domain routing overview.
5. [Creative/WorldBuilding/Franchises/StarshipShamples.md](../WorldBuilding/Franchises/StarshipShamples.md) when brand work touches LotAT continuity.
6. [Creative/Art/AGENTS.md](../Art/AGENTS.md) when brand work touches visual identity.
7. [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md) when brand work becomes campaign, social, or community growth execution.

## Local Workflow

1. Start from brand identity. The stream is a live R&D workspace and marketing engine; Starship Shamples is both the game layer and the metaphor for the neurodivergent creative experience.
2. Apply the brand pillars: authenticity, accessibility, neurodivergent celebration, community as crew, and chaos with purpose.
3. Keep voice chaotic, warm, self-aware, and genuinely knowledgeable. Specific and honest beats polished or aspirational.
4. Match the context:
   - stream game narration: dramatic with a knowing wink, short enough to read aloud
   - Twitch bot responses: one or two sentences, characterful, warm, not robotic
   - stream titles: specific, honest, slightly absurd, never clickbait
   - community posts: direct, warm, low-friction, participation-oriented
   - social captions: hook first, depth second, no manufactured hype
5. Preserve the core metaphor. The ship is the ADHD space-cadet mind; the chaotic crew and commanders are competing impulses and executive function; the ship still flying is the point.
6. For character decisions, use the [Character Codex](CHARACTER-CODEX.md) as the authority. Each character owns a distinct personality and metaphor space.
7. If the work becomes community growth, clip strategy, campaign planning, or platform formatting, hand off through [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md).

## Validation

For brand-doc or local agent-guide changes, run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-09-validator.failures.txt
```

For public copy, validate by checklist:

- The copy is specific to what is actually happening.
- It is warm toward the community and does not shame chaos, failure, missed timing, or unfinished work.
- It avoids generic hype, clickbait, corporate polish, and luxury/aspirational positioning.
- It treats the audience as participating crew, not passive consumers.
- Character voices match the [Character Codex](CHARACTER-CODEX.md).

For canon-sensitive changes, complete or request [canon-guardian](../../.agents/workflows/canon-guardian.md) review.

## Boundaries / Out of Scope

Do not use this folder to:

- plan platform-specific posting schedules or caption packages without [Creative/Marketing/AGENTS.md](../Marketing/AGENTS.md)
- write full LotAT adventures without [Creative/WorldBuilding/AGENTS.md](../WorldBuilding/AGENTS.md)
- generate visual prompts without [Creative/Art/AGENTS.md](../Art/AGENTS.md)
- introduce new named cast members, permanent ship sections, or metaphor remaps without operator approval
- turn the community into a public-facing funnel or conversion target

## Handoff Notes

Brand handoffs should include:

- public context: chat, title, announcement, community post, story narration, marketing, art, or canon review
- source docs read from [Creative/Brand/](./)
- voice constraints applied
- canon layer classification: no canon impact, local flavor, reusable LotAT lore, or franchise-level canon
- whether [canon-guardian](../../.agents/workflows/canon-guardian.md) was run or is required
- any chain-to role needed for implementation or distribution

Canon-sensitive content requiring operator review includes new named characters, changes to personality or metaphor roles, permanent world elements, new story-authored mechanics, and any edit that would require changing [Creative/Brand/CHARACTER-CODEX.md](CHARACTER-CODEX.md).
