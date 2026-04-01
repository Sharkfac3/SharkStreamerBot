# Core Skills — lotat-writer

## Narrative Principles

- Stage narration: **1–4 sentences**. This is live stream content — keep it fast.
- Editorial target: **12+ stage nodes** for a full mission unless a shorter format is explicitly intended.
- Each **stage** node may offer **1 or 2 choices** in v1; prefer 2 when the stage benefits from contrast and replayability.
- Branching variety: avoid linear chains; multiple paths should lead to unique outcomes.
- Commander moments are **rare** — editorial target of 1–2 per story maximum.
- Dice hooks add tension — use sparingly, not as a default mechanic.
- Ending nodes should reflect the final chaos level in tone.

## The Separation of Concerns

The story agent produces content. The technical agent produces code. Never mix them.

- Story JSON is the output of this role
- Do not write C# in story files
- Do not reference engine implementation details in narrative
- The authoritative story-file contract lives in `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
- `lotat-writer` owns story content inside that contract and owns pre-review validation that a story is safe to hand to the reviewer/runtime path, but does not own schema changes
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

- Generated stories must always be written to a new file at `Creative/WorldBuilding/Storylines/drafts/<story_id>.json` unless the operator explicitly asks for outline-only output
- Do not leave a generated story only in chat output; file creation in `drafts/` is the default requirement
- Inline JSON may be shown for review if requested, but it does not replace writing the draft file
- Do not place stories directly in `Creative/WorldBuilding/Storylines/ready/`
- Before a story is handed to the reviewer/runtime path, run the writer-side validation pass and treat engine-breaking defects as hard-fatal
- To review: launch the story viewer with `./run-lotat-story-viewer.sh` from the repo root, then open `http://localhost:8000`
- Stories move to `ready/` when the operator clicks Handoff in the viewer
- `ready/` is the source for all downstream agents — do not modify a story after handoff

## Validation Responsibility

The writer role is responsible for catching engine-breaking story defects before handoff.

### Hard-fatal before reviewer/runtime handoff

Treat these as hard-fatal before reviewer/runtime handoff:
- malformed JSON / parse failure
- missing required top-level fields or node fields
- missing or unresolved `starting_node_id`
- duplicate `node_id`
- duplicate `choice_id`
- invalid `node_type` or `end_state`
- invalid authored decision commands
- runtime-only commands placed in authored story-choice fields
- `next_node_id` references to missing nodes
- stage nodes with zero choices in v1
- stage nodes with more than 2 choices in v1
- malformed ending nodes
- malformed commander-moment payloads
- malformed dice-hook payloads
- related graph defects that could break runtime execution

### Warning-only in v1

Warnings are low priority in v1 unless the story can still run safely. If an issue could break the engine, it is not a warning.

### Editorial guidance

The following are useful quality targets, but they are not engine-safety validation failures by themselves:
- aiming for a full mission length around 12+ stage nodes
- preferring 2 choices when a stage benefits from strong contrast
- using commander moments and dice hooks sparingly
- making sure Pedro worsens at least one problem for franchise flavor

## Business Context

LotAT stories entertain viewers during a live R&D stream about building off-road racing products. The stories you write serve two purposes: keeping live viewers engaged during slow stretches, and creating memorable moments that become short-form content (YouTube Shorts, TikTok, etc.) for broader reach. Write for both contexts — make moments that are entertaining live AND that would make someone stop scrolling. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Tone

Absurd, chaotic, slightly dramatic, fast-paced, humorous, failure-forward. The brand celebrates chaos — do not introduce darkness or seriousness that does not serve humor.
