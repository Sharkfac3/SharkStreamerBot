# LotAT — streamerbot-dev Scope

## What This Is

Legends of the ASCII Temple (LotAT) is the interactive D&D-style adventure system that runs on stream. From the `streamerbot-dev` perspective, this folder covers the **C# runtime engine** — the Streamer.bot actions that execute story nodes, handle chat commands, manage state, and drive the adventure forward.

## Current Status

Reserved / in-progress. The `Actions/LotAT/` folder is the target location for all LotAT C# engine scripts.

## Role Boundary

| Concern | Role |
|---|---|
| C# engine that runs story nodes | `streamerbot-dev` (here) |
| Story JSON schema, pipeline architecture | `lotat-tech` |
| Narrative content, adventure design, lore | `lotat-writer` |

Do not mix concerns. If a task is "write a new adventure," use `lotat-writer`. If it is "implement the engine command that executes node transitions," use `streamerbot-dev` here + `lotat-tech` for schema context.

## LotAT State Variables

The offering system in `Actions/Squad/offering.cs` already interacts with LotAT state. Key variables (from `Actions/SHARED-CONSTANTS.md`):
- `lotat_active`
- `lotat_announcement_sent`
- `lotat_offering_steal_chance`
- `boost_*`

Any new LotAT global variable must be reset in `Actions/Twitch Core Integrations/stream-start.cs` and added to `Actions/SHARED-CONSTANTS.md`.

## When This Expands

As the LotAT engine is built out, add specific skill files here (e.g., `engine-commands.md`, `node-runner.md`). Update this index when new scripts are added to `Actions/LotAT/`.
