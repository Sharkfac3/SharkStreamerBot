# Core Skills — lotat-writer

## Narrative Principles

- Stage narration: **1–4 sentences**. This is live stream content — keep it fast.
- Minimum story length: **12 stages**
- Each stage offers **exactly 2 choices**
- Branching variety: avoid linear chains; multiple paths should lead to unique outcomes
- Commander moments are **rare** — 1–2 per story maximum
- Dice hooks add tension — use sparingly, not as a default mechanic
- Terminal nodes (endings) must reflect the final Chaos Meter level in tone

## The Separation of Concerns

The story agent produces content. The technical agent produces code. Never mix them.

- Story JSON is the output of this role
- Do not write C# in story files
- Do not reference engine implementation details in narrative
- If a story needs a new command, flag it for `lotat-tech` — do not invent it in the story

## Canon Rules — Non-Negotiable

- The cast is fixed — do not invent new named cast members without explicit operator approval
- Do not change established character personalities
- Do not reset the Chaos Meter mid-story
- Each story's `commands_used` must only contain commands from the supported list (see `lotat-tech/skills/engine/commands.md`)
- Do not add top-level JSON schema fields not defined in the schema contract

## Required Reading Before Writing

| Document | Why |
|---|---|
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Story agent prompt — JSON schema, tone, mechanics rules |
| `Creative/WorldBuilding/Agents/D&D-Agent.md` | Game mechanics, Chaos Meter, dice system, tone |
| `Creative/Brand/CHARACTER-CODEX.md` | Canonical character personalities — do not deviate |
| `Creative/Brand/BRAND-IDENTITY.md` | Brand metaphor layer — all content must be consistent |

## Business Context

LotAT stories entertain viewers during a live R&D stream about building off-road racing products. The stories you write serve two purposes: keeping live viewers engaged during slow stretches, and creating memorable moments that become short-form content (YouTube Shorts, TikTok, etc.) for broader reach. Write for both contexts — make moments that are entertaining live AND that would make someone stop scrolling. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Tone

Absurd, chaotic, slightly dramatic, fast-paced, humorous, failure-forward. The brand celebrates chaos — do not introduce darkness or seriousness that does not serve humor.
