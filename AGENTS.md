# AGENTS.md

## Project Context

This repository contains **Streamer.bot C# action scripts** and local **BuildTools** utilities. Scripts are **not auto-deployed** — each is manually copy/pasted into Streamer.bot actions.

Repository hierarchy: `Actions/<Feature Group>/<Subfeature>/script.cs`

### Feature Groups
- `Actions/Squad/` — Chat mini-games (Clone, Duck, Pedro, Toothless, offering)
- `Actions/Commanders/` — Commander role assignment + commander-specific commands
- `Actions/Twitch Integration/` — Stream lifecycle, Bits forwarding, core Twitch glue
- `Actions/LotAT/` — Reserved / in-progress
- `BuildTools/` — External terminal-run utilities (not pasted into Streamer.bot)

### Key References
- `Actions/SHARED-CONSTANTS.md` — Canonical global variable / OBS / timer names
- `Actions/HELPER-SNIPPETS.md` — Reusable C# copy/paste patterns

---

## Skill Routing

Load the listed skills based on the task at hand. `streamerbot-scripting` is the base skill for **all** `.cs` work. `change-summary` is a **terminal skill** — always load it after completing code changes.

| Task | Skills to load |
|---|---|
| Writing/editing any `.cs` script | `streamerbot-scripting` + relevant `feature-*` |
| Squad mini-game work | `streamerbot-scripting` + `feature-squad` |
| Commander role/command work | `streamerbot-scripting` + `feature-commanders` |
| Bits / stream-start / Twitch glue | `streamerbot-scripting` + `feature-twitch-integration` |
| BuildTools / MixItUp / WorldBuilding | `buildtools` |
| Preparing change summary / paste targets | `change-summary` |
| Syncing repo to Streamer.bot | `sync-workflow` |

### Chaining Example

> "Fix the Duck mini-game cooldown" →
> Load `streamerbot-scripting` → `feature-squad` → (make changes) → `change-summary`

---

## Default Priority Order

1. Live stream reliability
2. Safe chat-facing behavior
3. Backward compatibility for existing features
4. Maintainable, readable scripts
5. Minimal operator friction during manual copy/paste sync

---

## Scope Boundaries

**In scope:**
- Implement/maintain C# scripts for Streamer.bot actions.
- Implement/maintain local tooling under `BuildTools/`.
- Focused, minimal-risk fixes.
- Readability/reliability improvements without changing intended behavior.
- Thorough, beginner-friendly comments.

**Out of scope (unless explicitly requested):**
- Broad refactors across unrelated feature groups.
- Renaming triggers/behavior that chat depends on.
- Changing core stream-start/housekeeping behavior.
- Introducing unnecessary external dependencies.
