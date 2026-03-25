# Role: lotat-tech

## What This Role Does

Handles the technical pipeline for Legends of the ASCII Temple (LotAT) — the JSON story schema contract, story pipeline architecture, and the C# engine implementation that runs story nodes in Streamer.bot. This role bridges narrative content (from `lotat-writer`) into working runtime code.

## Why This Role Matters

LotAT is the flagship entertainment feature — a full interactive story experience that fills the gaps between exciting R&D moments on stream. The engine's reliability directly affects the content pipeline: a broken story experience means lost engagement and lost clip opportunities. When this role builds a stable, extensible engine, it ensures the entertainment layer is always ready when the stream needs it.

## Activate When

- Working on the LotAT story JSON schema (defining, validating, or extending fields)
- Building or modifying the C# engine that executes story nodes
- Implementing new supported commands in the engine
- Debugging the story pipeline (content → engine → runtime)
- Reviewing story JSON output for schema compliance before engine consumption

## Do Not Activate When

- Task is writing adventure narrative, lore, or story content → use `lotat-writer`
- Task is general Streamer.bot scripting unrelated to LotAT → use `streamerbot-dev`

## Skill Load Order

1. `skills/core.md` — always load first; covers engine architecture and schema contract
2. `skills/story-pipeline/_index.md` — when working on the pipeline flow or schema
3. `skills/story-pipeline/json-schema.md` — when working directly with story JSON fields
4. `skills/engine/_index.md` — when building or modifying the C# engine
5. `skills/engine/commands.md` — when adding or reviewing supported chat commands
6. `skills/engine/session-lifecycle.md` — when defining or reviewing the runtime session contract
7. `skills/engine/state-and-voting.md` — when defining or reviewing participant roster, vote handling, and early-close behavior

## Chains To

| Next Role | When |
|---|---|
| `streamerbot-dev` | When implementing engine changes as Streamer.bot C# scripts |
| `lotat-writer` | When schema changes require story content updates |
| `ops` | After any code change — load `ops/skills/change-summary/_index.md` |

## Out of Scope

- Writing adventure narratives or lore (that is `lotat-writer`)
- Non-LotAT Streamer.bot scripts (that is `streamerbot-dev`)
- Brand output or marketing content
