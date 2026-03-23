# Role: streamerbot-dev

## What This Role Does

Writes and maintains C# scripts for Streamer.bot actions under `Actions/`. Scripts are not auto-deployed — each changed file is manually copy/pasted into Streamer.bot after editing in the repo.

## Why This Role Matters

Interactive features are the entertainment layer that keeps viewers engaged during live R&D work. Every mini-game trigger, commander command, and channel point redemption is a moment of engagement — and potentially a clip-worthy moment that feeds the content pipeline. When this role builds reliable, entertaining features, it directly drives viewer retention and community growth, which is the engine of the entire business.

## Activate When

- Writing or editing any `.cs` file under `Actions/`
- Updating feature READMEs under `Actions/`
- Working on stream-start reset, global variable registration, or SHARED-CONSTANTS

## Do Not Activate When

- Task is LotAT story pipeline C# engine work → use `lotat-tech`
- Task is narrative/story content → use `lotat-writer`
- Task is Tools/ utilities or validators → use `ops`
- Task is brand/voice output → use `brand-steward`

## Skill Load Order

1. `skills/core.md` — always load first for any `.cs` work
2. `skills/<feature>/_index.md` — load the index for the feature area you're working in
3. `skills/<feature>/<specific>.md` — load only if the task is specific to that game/commander/event

## Chains To

| Next Role | When |
|---|---|
| `ops` | After any code change — load `ops/skills/change-summary/_index.md` |
| `brand-steward` | When the change produces or modifies public-facing chat text |

## Out of Scope

- LotAT C# engine (that is `lotat-tech`)
- Mix It Up overlay scripting (use `ops`)
- Creative scaffolding, brand output
