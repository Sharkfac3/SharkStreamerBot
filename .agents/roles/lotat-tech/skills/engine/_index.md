# LotAT Engine — Overview

## What the Engine Does

The C# engine running in Streamer.bot actions under `Actions/LotAT/`:
- Loads and parses story JSON
- Tracks current node state
- Handles chat command routing to story mechanics
- Advances the story based on chat votes/commands
- Manages the Chaos Meter
- Triggers Mix It Up overlays for narration/effects

## Engine Design Principles

- Story content is **data** (JSON), not **code** — the engine should not contain hardcoded story text
- Engine is story-agnostic — any valid JSON story file should run without engine modifications
- Chat commands are the primary interaction surface — see `commands.md` for supported set
- Chaos Meter state is tracked in global variables and must survive action re-entry

## Current Status

`Actions/LotAT/` is reserved and in active development. As engine scripts are created, document each one here.

## State Variables

All LotAT engine state in `Actions/SHARED-CONSTANTS.md`:
- `lotat_active` — session running flag
- `lotat_announcement_sent` — session start dedup
- `lotat_offering_steal_chance` — offering integration
- `boost_*` — boost state

## Sub-Skills

- `commands.md` — supported chat commands and engine constraints
