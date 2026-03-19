# Core Skills — lotat-tech

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

## Schema is a Contract

The JSON story schema is shared between `lotat-writer` (produces it) and `lotat-tech` (consumes it). **Schema drift breaks the engine.**

- Do not add top-level fields without updating both the schema doc and the engine
- Do not rename existing fields without a migration plan
- Version bumps require operator approval

## State Management

LotAT engine state lives in Streamer.bot global variables. Key variables from `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — whether a session is currently running
- `lotat_announcement_sent` — deduplication flag for session start announcement
- `lotat_offering_steal_chance` — offering mechanic interaction
- `boost_*` — boost state variables

Any new LotAT state variable must be added to `stream-start.cs` reset and `Actions/SHARED-CONSTANTS.md`.

## Key References

| File | Why |
|---|---|
| `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | Technical agent prompt — engine implementation guidelines |
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Story agent prompt — defines the JSON schema the engine must consume |
| `Actions/SHARED-CONSTANTS.md` | LotAT global variable names |
| `Actions/HELPER-SNIPPETS.md` | Reusable C# patterns |
