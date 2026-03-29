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
- The runtime session contract is documented in `skills/engine/session-lifecycle.md` and `skills/engine/state-and-voting.md`; treat those as the navigation target for runtime-spec questions before discussing C# implementation details

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

### V1 runtime state contract direction

For v1, use a runtime state model that is **easy for coding agents to implement and reason about**:
- use **explicit individual globals** for scalar values such as flags, IDs, counts, stage, and active window type
- use **JSON-packed globals only for structured collections** such as the joined roster, allowed command lists, and vote map
- treat the JSON storage shapes in the engine runtime docs as **canonical**, not implementation-detail suggestions
- optimize for **normal action-to-action continuity** during live operation, not crash recovery
- keep the required state set **minimal** so the engine can work first and become more recovery-friendly later
- defer recovery snapshots, rich branch history, and vote-history retention beyond v1

Canonical v1 identity/storage rule:
- LotAT runtime user identity should be stored as a **lowercase username/login string**
- use that same lowercase username form consistently for joined-roster membership, active vote-map keys, and commander-target comparison
- do **not** introduce richer per-user JSON objects in v1 unless the runtime contract is intentionally expanded later

Recommended naming style for new runtime variables:
- preserve existing `lotat_active` for backward compatibility
- use clear category prefixes for new variables: `lotat_session_*`, `lotat_node_*`, and `lotat_vote_*`
- prefer literal names over abstract names so future coding agents are less likely to hallucinate meanings

Any new LotAT state variable must be added to `Actions/Twitch Core Integrations/stream-start.cs` reset and `Actions/SHARED-CONSTANTS.md` when implementation begins.

## Key References

| File | Why |
|---|---|
| `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | Technical agent prompt — engine implementation guidelines |
| `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | Story agent prompt — defines the JSON schema the engine must consume |
| `skills/engine/docs-map.md` | First-stop navigation map to the runtime session contract docs |
| `skills/engine/session-lifecycle.md` | Canonical runtime session flow spec |
| `skills/engine/state-and-voting.md` | Canonical runtime participation/voting spec |
| `Actions/SHARED-CONSTANTS.md` | LotAT global variable names |
| `Actions/HELPER-SNIPPETS.md` | Reusable C# patterns |
