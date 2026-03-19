# Project: SharkStreamerBot

## What It Is

A Twitch streaming platform centered on SharkFac3's streams. Built on Streamer.bot + Mix It Up + OBS. Includes an interactive D&D-style adventure system (Legends of the ASCII Temple / LotAT), brand identity, character art, and a growing set of stream interaction tools.

## Domains

| Domain | Path | Contents |
|---|---|---|
| Actions | `Actions/` | Streamer.bot C# runtime scripts — manually copy/pasted into Streamer.bot |
| Tools | `Tools/` | Local utilities, Mix It Up API helpers, validators, sync tools |
| Creative | `Creative/` | Brand docs, character codex, art agents, worldbuilding, lore |
| Docs | `Docs/` | Architecture, workflow, onboarding |
| Agent Tree | `.agents/` | This tree — shared role/skill knowledge |

## Key References

| File | Purpose |
|---|---|
| `WORKING.md` | Active agent work, task queue, conflict tracking — **check before starting any task** |
| `Actions/SHARED-CONSTANTS.md` | Canonical global variable, OBS source, and timer names |
| `Actions/HELPER-SNIPPETS.md` | Reusable C# patterns — copy verbatim, do not rewrite |
| `Creative/Brand/BRAND-IDENTITY.md` | Brand vision, mission, values, neurodivergent metaphor |
| `Creative/Brand/BRAND-VOICE.md` | Tone and language conventions per output context |
| `Creative/Brand/CHARACTER-CODEX.md` | Canonical character identities |
| `Docs/ONBOARDING.md` | Start here if new to the project |

## Priority Order

1. Live stream reliability
2. Safe chat-facing behavior
3. Backward compatibility for existing features
4. Maintainable, readable scripts
5. Minimal operator friction during manual copy/paste sync

## Scope Rules

**In scope:**
- Implement/maintain C# scripts under `Actions/`
- Implement/maintain local tooling under `Tools/`
- Implement/maintain creative scaffolding under `Creative/` when explicitly requested
- Focused, minimal-risk fixes
- Readability/reliability improvements without changing intended behavior
- Thorough, beginner-friendly comments

**Out of scope (unless explicitly requested):**
- Broad refactors across unrelated feature groups
- Renaming triggers/behaviors that chat depends on
- Changing core stream-start/housekeeping behavior
- Introducing unnecessary external dependencies
