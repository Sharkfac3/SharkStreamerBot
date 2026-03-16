---
name: skills
description: Overview of project skills, routing, and conventions for maintaining the .pi/skills directory.
---

# Pi Skills

This project uses [pi skills](https://agentskills.io/specification) to break agent instructions into focused, on-demand modules. Instead of loading all project rules into every conversation, pi loads only the skills relevant to the current task.

## Why Skills?

The original `AGENTS.md` was a single ~300-line file covering everything from C# scripting rules to commit formatting. Every task loaded all of it, regardless of relevance.

Now:
- `AGENTS.md` is a ~66-line **routing table** that maps tasks to skills.
- Each skill is a self-contained instruction set loaded only when needed.
- A typical task loads 2-3 skills (~120-150 lines) instead of the full 300+.

**No runtime code was changed.** This is purely an organizational change to how the agent receives instructions.

## Available Skills

| Skill | Description |
|---|---|
| `streamerbot-scripting` | Core C# scripting rules for Streamer.bot runtime. Compatibility, state management, safety, commenting standards, reusable patterns. **Base skill for all `.cs` work.** |
| `feature-squad` | Squad mini-games — Clone, Duck, Pedro, Toothless, offering. Action-folder map, shared constants, behavioral expectations. |
| `feature-commanders` | Commander role system — Captain Stretch, The Director, Water Wizard. 3-slot model, support commands, high score tracking. |
| `feature-voice-commands` | Voice-command mode and OBS scene switching actions. Canonical `stream_mode` values, scene naming, fallback rules. |
| `feature-twitch-integration` | Stream lifecycle — stream-start reset, Bits cheer forwarding, TTS wait behavior, idempotency rules. Includes Cheer, Follow, and Sub trigger variable reference. |
| `feature-hype-train` | Hype Train event scripts (start, level-up, end). Includes full trigger variable tables for all three events. |
| `feature-channel-points` | Channel Point redeem scripts (Disco Party, Explain Current Task). Includes reward redemption trigger variable reference. |
| `buildtools` | External tooling for `Tools/` work — Mix It Up API command discovery, Python utilities, StreamerBot validators. |
| `creative-art` | Art generation agents — character canon references, diffusion model prompt conventions, asset naming, output placement. |
| `creative-worldbuilding` | Lore, canon, and CYOA story generation — Starship Shamples franchise, cast canon, JSON story schema, story vs. engine separation. |
| `brand-steward` | Brand consistency for all public-facing output — voice, tone, values, and the neurodivergent metaphor. **Load before writing any chat text, stream titles, or community messaging.** |
| `brand-canon-guardian` | Audits new content against established Starship Shamples canon. Load when adding new characters, extending lore, or reviewing stories for consistency. |
| `content-strategy` | Connects Starship Shamples story content to real build sessions. Load when planning stories tied to a specific project or writing build-specific stream content. |
| `sync-workflow` | Repo-to-Streamer.bot paste process, validation checklists, commit note style. |
| `change-summary` | Standard response format for code changes — paste targets, setup steps, validation output. **Terminal skill — loaded after every code change.** |

## Routing

`AGENTS.md` contains the routing table that maps tasks to skills. Examples:

| Task | Skills loaded |
|---|---|
| Fix a Duck mini-game bug | `streamerbot-scripting` → `feature-squad` → `change-summary` |
| Add a new Commander command | `streamerbot-scripting` → `feature-commanders` → `change-summary` |
| Update stream-start reset | `streamerbot-scripting` → `feature-twitch-integration` → `change-summary` |
| Work on Hype Train scripts | `streamerbot-scripting` → `feature-hype-train` → `change-summary` |
| Work on Channel Point redeems | `streamerbot-scripting` → `feature-channel-points` → `change-summary` |
| Modify MixItUp API script | `buildtools` → `change-summary` |
| Generate character art prompts | `creative-art` |
| Write a new Starship Shamples story | `creative-worldbuilding` → `brand-canon-guardian` |
| Write a story tied to a specific build session | `content-strategy` → `creative-worldbuilding` → `brand-canon-guardian` |
| Full story pipeline (content + C# engine) | `creative-worldbuilding` → `streamerbot-scripting` → `change-summary` |
| Write chat bot output text (follow, sub, bits, raid) | `brand-steward` |
| Write stream titles or community posts | `brand-steward` |
| Review new lore or character for canon | `brand-canon-guardian` |
| Prepare a sync to Streamer.bot | `sync-workflow` |

## Adding a New Skill

1. Create a folder under `.pi/skills/<skill-name>/`.
2. Add a `SKILL.md` with required frontmatter (`name`, `description`) and instructions.
3. Add a row to the routing table in `AGENTS.md`.
4. Add a row to the table above.

### Naming Rules
- Lowercase letters, numbers, hyphens only.
- No leading/trailing hyphens, no consecutive hyphens.
- Folder name must match the `name` field in frontmatter.
