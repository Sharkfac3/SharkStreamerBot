# LotAT — streamerbot-dev Scope

## What This Is

Legends of the ASCII Temple (LotAT) is the interactive D&D-style adventure system that runs on stream. From the `streamerbot-dev` perspective, this folder covers the **C# runtime engine** — the Streamer.bot actions that open the session join phase, register `!join` participants, execute story nodes, handle chat commands, manage state, and drive the adventure forward.

## Current Status

Reserved / in-progress. The `Actions/LotAT/` folder is the target location for all LotAT C# engine scripts.

## Role Boundary

| Concern | Role |
|---|---|
| C# engine that runs story nodes | `streamerbot-dev` (here) |
| Runtime contract docs for session flow, roster rules, and vote handling | `lotat-tech` |
| Story JSON schema, pipeline architecture | `lotat-tech` |
| Narrative content, adventure design, lore | `lotat-writer` |

Do not mix concerns. If a task is "write a new adventure," use `lotat-writer`. If it is "implement the engine command that executes node transitions," use `streamerbot-dev` here + `lotat-tech` for schema/runtime-contract context.

## LotAT State Variables

The offering system in `Actions/Squad/offering.cs` already interacts with LotAT state. Key variables (from `Actions/SHARED-CONSTANTS.md`):
- `lotat_active`
- `lotat_announcement_sent`
- `lotat_offering_steal_chance`
- `boost_*`

Runtime behavior expected by the current LotAT contract:
- session start opens a join phase before the first story node decision
- `!join` registers a viewer into the session participant roster
- later decision windows may auto-close once every joined participant has submitted one of the allowed node commands

Before implementing LotAT runtime behavior in `Actions/`, read these lotat-tech engine docs in order:
- `.agents/roles/lotat-tech/skills/engine/docs-map.md`
- `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
- `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
- `.agents/roles/lotat-tech/skills/engine/commands.md`

Treat those lotat-tech docs as the runtime contract source. This `streamerbot-dev` index points implementers back to the contract; it does not redefine session rules locally.

Any new LotAT global variable must be reset in `Actions/Twitch Core Integrations/stream-start.cs` and added to `Actions/SHARED-CONSTANTS.md`.

## When This Expands

As the LotAT engine is built out, add specific skill files here (e.g., `engine-commands.md`, `node-runner.md`). Update this index when new scripts are added to `Actions/LotAT/`.
