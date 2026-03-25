# Core Skills — lotat-writer

## Narrative Principles

- Stage narration: **1–4 sentences**. This is live stream content — keep it fast.
- Minimum story length: **12 stage nodes**
- Each **stage** node offers **exactly 2 choices**
- Branching variety: avoid linear chains; multiple paths should lead to unique outcomes
- Commander moments are **rare** — 1–2 per story maximum
- Dice hooks add tension — use sparingly, not as a default mechanic
- Ending nodes must reflect the final chaos level in tone

## The Separation of Concerns

The story agent produces content. The technical agent produces code. Never mix them.

- Story JSON is the output of this role
- Do not write C# in story files
- Do not reference engine implementation details in narrative
- The authoritative story-file contract lives in `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
- `lotat-writer` owns story content inside that contract, but does not own schema changes
- If field-level certainty matters, defer to the authoritative contract rather than any summary doc
- If a story needs a new command, field, or contract change, flag it for `lotat-tech` — do not invent it in the story
- Canon, cast, or metaphor changes escalate to `brand-steward`

## Canon Rules — Non-Negotiable

- The cast is fixed — do not invent new named cast members without explicit operator approval
- Do not change established character personalities
- Do not reset the Chaos Meter mid-story
- Each story's `commands_used` must only contain authored decision commands from the supported list (see `lotat-tech/skills/engine/commands.md`)
- Runtime session commands like `!join` are engine behavior and must not be placed in `commands_used` or `choices[].command`
- Use the contract's actual top-level `cast` object with `commanders_used` and `squad_members_used`
- Do not add top-level or node-level JSON fields not defined in the story contract

## Required Reading Before Writing

| Document | Why |
|---|---|
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Authoritative story contract — JSON structure, tone, mechanics rules |
| `Creative/WorldBuilding/Agents/D&D-Agent.md` | Game mechanics, Chaos Meter, dice system, tone |
| `Creative/Brand/CHARACTER-CODEX.md` | Canonical character personalities — do not deviate |
| `Creative/Brand/BRAND-IDENTITY.md` | Brand metaphor layer — all content must be consistent |

## Story File Workflow

- Write draft stories to `Creative/WorldBuilding/Storylines/drafts/<story_id>.json`
- Do not place stories directly in `Creative/WorldBuilding/Storylines/ready/`
- To review: launch the story viewer with `./run-lotat-story-viewer.sh` from the repo root, then open `http://localhost:8000`
- Stories move to `ready/` when the operator clicks Handoff in the viewer
- `ready/` is the source for all downstream agents — do not modify a story after handoff

## Business Context

LotAT stories entertain viewers during a live R&D stream about building off-road racing products. The stories you write serve two purposes: keeping live viewers engaged during slow stretches, and creating memorable moments that become short-form content (YouTube Shorts, TikTok, etc.) for broader reach. Write for both contexts — make moments that are entertaining live AND that would make someone stop scrolling. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Tone

Absurd, chaotic, slightly dramatic, fast-paced, humorous, failure-forward. The brand celebrates chaos — do not introduce darkness or seriousness that does not serve humor.
