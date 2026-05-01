---
id: shared-project
type: shared
description: Repo-wide project identity, domains, priority order, and scope rules.
status: active
owner: ops
---

# Project: SharkStreamerBot

## Business Context

SharkFac3 is building an R&D company focused on off-road racing products in public. The live stream is the workspace and the marketing engine: products are researched and developed live, knowledge is shared freely to build authority, and entertainment layers keep the community engaged during slow stretches.

## Content Model

- Live stream: real R&D work plus interaction systems.
- Entertainment layers: commanders, mini-games, overlays, and LotAT stories that create engagement and clip-worthy moments.
- Short-form pipeline: highlights become YouTube Shorts, TikTok, Instagram Reels, and related discovery content.
- Product path: the community that watched the products develop can become the eventual customer base.

## Repository Domains

| Domain | Guide | Contents |
|---|---|---|
| Actions | [Actions/](../../Actions/) | Streamer.bot C# runtime scripts, manually pasted into Streamer.bot. |
| Apps | [Apps/](../../Apps/) | Standalone TypeScript apps: stream overlay, info-service, and production manager. |
| Tools | [Tools/](../../Tools/) | Local utilities, validators, Mix It Up helpers, and sync tooling. |
| Creative | [Creative/](../../Creative/) | Brand docs, character art, marketing, worldbuilding, lore, and creative scaffolding. |
| Agent Tree | [../](../) | Agent roles, workflows, manifest routing, and repo-wide shared context. |

## Key References

| File | Purpose |
|---|---|
| [Actions/SHARED-CONSTANTS.md](../../Actions/SHARED-CONSTANTS.md) | Canonical global variable, OBS source, and timer names. |
| [Actions/Helpers/AGENTS.md](../../Actions/Helpers/AGENTS.md) | Reusable C# patterns. |
| [Creative/Brand/BRAND-IDENTITY.md](../../Creative/Brand/BRAND-IDENTITY.md) | Brand vision, mission, values, and neurodivergent metaphor. |
| [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) | Tone and language conventions by output context. |
| [Creative/Brand/CHARACTER-CODEX.md](../../Creative/Brand/CHARACTER-CODEX.md) | Canonical character identities. |

## Priority Order

1. Live stream reliability.
2. Safe chat-facing behavior.
3. Content pipeline utility.
4. Knowledge-sharing value.
5. Backward compatibility for existing features.
6. Maintainable, readable scripts.
7. Minimal operator friction during manual sync/paste.

## Scope Rules

In scope: focused maintenance of Streamer.bot actions, TypeScript apps, local tooling, creative scaffolding when requested, and agent documentation/routing.

Out of scope unless explicitly requested: broad unrelated refactors, behavior renames that chat depends on, core stream-start/housekeeping changes, or unnecessary external dependencies.
