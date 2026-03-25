# Core Skills — lotat-tech

## Business Context

The LotAT engine powers the primary entertainment feature for a live R&D stream. Viewers watch real product development for an off-road racing company — LotAT fills the inevitable slow stretches with interactive storytelling that keeps them engaged and creates clip-worthy moments. Engine reliability equals content pipeline reliability. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## The Two-Layer Architecture

LotAT has a strict separation of concerns:

| Layer | Produces | Consumes |
|---|---|---|
| Story Agent (`lotat-writer`) | Narrative + JSON story file | Game design agent, universe canon |
| Technical Agent (`lotat-tech`) | C# Streamer.bot engine | Story JSON from `lotat-writer` |

**Do not mix layers in a single output.** If the task is "write a new adventure AND implement it," complete the story JSON first, then chain to `streamerbot-dev` for the C# implementation.

## Engine Architecture Principles

- The C# engine consumes story JSON — it does not contain story content
- Engine reads node data, executes commands, tracks state, advances the story
- Story content (narration, choices, character moments) lives entirely in JSON
- Engine commands are the bridge between JSON story events and Streamer.bot actions
- Session lifecycle behavior such as join windows, participant roster tracking, and vote auto-close rules lives in the engine/runtime layer, not in authored story JSON

## Schema is a Contract

The JSON story schema is shared between `lotat-writer` (produces it) and `lotat-tech` (consumes it). **Schema drift breaks the engine.**

- The authoritative authored-story contract lives in `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
- `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` is a synced implementation summary, not a competing authority
- `lotat-tech` owns schema and command-contract changes
- `lotat-writer` owns story content within the schema
- Canon, cast, or metaphor changes escalate to `brand-steward`
- Do not add top-level fields without updating the authoritative contract, synced summaries, and engine plan together
- Do not rename existing fields without a migration plan
- Version bumps require operator approval

## State Management

LotAT engine state lives in Streamer.bot global variables. Key variables from `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — whether a session is currently running
- `lotat_announcement_sent` — deduplication flag for session start announcement
- `lotat_offering_steal_chance` — offering mechanic interaction
- `lotat_steal_multiplier` — offering steal scaling
- `boost_*` — boost state variables

Runtime design assumptions to preserve:
- each LotAT session begins with a join phase before the first story decision
- viewers opt into that session with `!join`
- the engine tracks a per-session joined-user roster
- during a decision window, the engine may close voting early once every joined user has submitted one of the currently allowed decision commands
- join roster data is runtime state, not authored story schema

Any new LotAT state variable must be added to `Actions/Twitch Core Integrations/stream-start.cs` reset and `Actions/SHARED-CONSTANTS.md`.

## Key References

| File | Why |
|---|---|
| `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | Technical agent prompt — engine implementation guidelines |
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Story agent prompt — defines the JSON schema the engine must consume |
| `Actions/SHARED-CONSTANTS.md` | LotAT global variable names |
| `Actions/HELPER-SNIPPETS.md` | Reusable C# patterns |
