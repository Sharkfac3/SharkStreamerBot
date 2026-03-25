# LotAT Engine — Overview

## What the Engine Does

The C# engine running in Streamer.bot actions under `Actions/LotAT/`:
- Loads and parses story JSON
- Opens a session-start join phase and registers `!join` participants
- Tracks current node state
- Handles chat command routing to story mechanics
- Advances the story based on chat votes/commands
- Closes a decision window early when all joined participants have voted
- Manages the Chaos Meter
- Triggers Mix It Up overlays for narration/effects

## Engine Design Principles

- Story content is **data** (JSON), not **code** — the engine should not contain hardcoded story text
- Engine is story-agnostic — any valid JSON story file should run without engine modifications
- Chat commands are the primary interaction surface — see `commands.md` for supported set
- Chaos Meter state is tracked in global variables and must survive action re-entry
- Runtime contract docs in this folder describe behavior boundaries; implementation work in `Actions/LotAT/` should follow them rather than move those rules into ad hoc script-specific docs

## Current Status

`Actions/LotAT/` is reserved and in active development. As engine scripts are created, document each one here.

## Runtime Session Spec — Start Here

If your question is about how a live LotAT run behaves before or between any future C# implementation details, read these docs in order:
1. `docs-map.md` — navigation entry point
2. `session-lifecycle.md` — canonical runtime session flow spec
3. `state-and-voting.md` — canonical runtime roster/voting spec
4. `commands.md` — runtime vs authored command boundary

These docs define the **runtime contract**. Future Streamer.bot scripts should implement them, not silently redefine them.

## State Variables

All LotAT engine state in `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — session running flag
- `lotat_announcement_sent` — session start dedup
- `lotat_offering_steal_chance` — offering integration
- `lotat_steal_multiplier` — offering steal scaling
- `boost_*` — boost state

Expected runtime state to add when the engine is implemented/refined:
- join-phase status for the active session
- joined-participant roster for the active session
- per-node vote submissions keyed to joined participants
- decision-window completion state so the engine can auto-close once all joined participants have voted

## Sub-Skills

- `docs-map.md` — high-level navigation map for the LotAT engine docs in this folder
- `commands.md` — supported chat commands and engine constraints
- `session-lifecycle.md` — canonical runtime session contract: stages, join flow, roster freeze, zero-join handling, node-entry flow, decision flow, teardown, and operator recovery controls
- `state-and-voting.md` — participant roster, vote storage, early-close rules, tie-break behavior, and edge cases
