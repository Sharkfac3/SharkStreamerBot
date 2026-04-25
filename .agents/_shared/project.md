# Project: SharkStreamerBot

## Business Context

**The Business:**
- SharkFac3 is building an R&D company focused on developing new products for off-road racing
- Products are researched and developed live on stream — the stream IS the workspace
- Knowledge is shared freely and openly as a strategy to establish authority in the off-road racing space
- This is a long-term play: build community through free knowledge → establish authority → sell products the community watched get developed

**The Content Pipeline:**
- **Live stream** — the primary content; real R&D work happening live with entertainment layers (mini-games, commanders, LotAT stories) keeping viewers engaged during slow stretches
- **Recorded highlights** — clip-worthy moments from stream (both technical breakthroughs and entertainment moments)
- **Short-form content** — highlights edited into YouTube Shorts, TikTok, Instagram Reels for broader reach
- **Community building** — short-form drives discovery, live stream builds loyalty, Discord deepens engagement
- **Product sales** — the community that watched products get developed becomes the customer base

**Why Entertainment Matters:**
- This is not a gaming stream — it's a company-building stream with entertainment layers
- Viewers are unlikely to identify with a company's success directly, so we entertain them while they watch the work happen
- Mini-games, commanders, and LotAT stories fill the gaps between exciting R&D moments
- Entertainment features should create clip-worthy moments that feed the content pipeline

## What It Is

SharkFac3's R&D company streams are powered by a Twitch streaming platform built on Streamer.bot + Mix It Up + OBS. This repo contains the streaming infrastructure: C# runtime scripts, an interactive D&D-style adventure system (Legends of the ASCII Temple / LotAT), brand identity, character art, and a growing set of stream interaction tools — all designed to keep viewers engaged while real work happens on camera.

## Domains

| Domain | Path | Contents |
|---|---|---|
| Actions | `Actions/` | Streamer.bot C# runtime scripts — manually copy/pasted into Streamer.bot |
| Apps | `Apps/` | Standalone TypeScript apps: `stream-overlay/` (Phaser OBS source + broker), `info-service/` (REST API, port 8766), `production-manager/` (React admin UI, port 5174) |
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
3. Content pipeline utility (does this create or support clip-worthy, repurposable content?)
4. Knowledge-sharing value (does this help share technical knowledge with the audience?)
5. Backward compatibility for existing features
6. Maintainable, readable scripts
7. Minimal operator friction during manual copy/paste sync

## Scope Rules

**In scope:**
- Implement/maintain C# scripts under `Actions/`
- Implement/maintain TypeScript apps under `Apps/`
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
