# AGENTS.md

## Project Context

This repository is organized into four top-level domains:
- `Actions/` — Streamer.bot C# runtime action source
- `Tools/` — external/local utilities and integration tooling
- `Creative/` — art, world-building, marketing, and related scaffolding
- `Docs/` — architecture and workflow documentation

Scripts are **not auto-deployed** — each changed `Actions/` script is manually copy/pasted into Streamer.bot actions.

Repository hierarchy for runtime scripts: `Actions/<Feature Group>/script.cs` or `Actions/<Feature Group>/<Action Folder>/script.cs`

### Feature Groups
- `Actions/Squad/` — Chat mini-games (Clone, Duck, Pedro, Toothless, offering)
- `Actions/Commanders/` — Commander role assignment + commander-specific commands
- `Actions/Twitch Core Integrations/` — Stream start plus follow/subscription event integrations stored directly in the feature-group folder
- `Actions/Twitch Channel Points/` — Channel point redeem scripts like Disco Party and Explain actions
- `Actions/Twitch Hype Train/` — Hype train start, level-up, and end scripts
- `Actions/Twitch Bits Integrations/` — Bits tier handlers and automatic bits reward integrations
- `Actions/Voice Commands/` — Voice-command-driven mode and scene switching actions
- `Actions/LotAT/` — Reserved / in-progress

### Routing notes
- Keep `Actions/` focused on Streamer.bot runtime source only.
- Mirror Streamer.bot action-group limits in the repo: feature-group folders under `Actions/` may contain scripts directly or one level of action folders, but those action folders should not contain additional subfolders.
- Use `Tools/` for Mix It Up scripts, overlays, sync helpers, validators, and similar local tooling.
- Use `Creative/` for art-generation, world-building, and other non-runtime scaffolding.

### Key References
- `Actions/SHARED-CONSTANTS.md` — Canonical global variable / OBS / timer names
- `Actions/HELPER-SNIPPETS.md` — Reusable C# copy/paste patterns

### Mix It Up API Payload Convention (Run Command)
When calling `POST /api/v2/commands/{commandId}` from Streamer.bot scripts:
- Keep standard fields: `Platform`, `Arguments`, `IgnoreRequirements`.
- Pass extra variables for Mix It Up command usage inside `SpecialIdentifiers` (not as top-level fields).
- Example payload shape:
  - `Platform = "Twitch"`
  - `Arguments = "optional message text"`
  - `SpecialIdentifiers = new { test = "True" }`
  - `IgnoreRequirements = false`

---

## Skill Routing

Load the listed skills based on the task at hand. `streamerbot-scripting` is the base skill for **all** `.cs` work. `change-summary` is a **terminal skill** — always load it after completing code changes.

| Task | Skills to load |
|---|---|
| Writing/editing any `.cs` script | `streamerbot-scripting` + relevant `feature-*` |
| Squad mini-game work | `streamerbot-scripting` + `feature-squad` |
| Commander role/command work | `streamerbot-scripting` + `feature-commanders` |
| Voice command mode/scene work | `streamerbot-scripting` + `feature-voice-commands` |
| Bits / stream-start / Twitch glue | `streamerbot-scripting` + `feature-twitch-integration` |
| `Tools/` or `Creative/` work (Mix It Up / local tooling / WorldBuilding scaffolding) | `buildtools` |
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
- Implement/maintain C# scripts for Streamer.bot actions under `Actions/`.
- Implement/maintain local tooling under `Tools/`.
- Implement/maintain creative scaffolding under `Creative/` when explicitly requested.
- Focused, minimal-risk fixes.
- Readability/reliability improvements without changing intended behavior.
- Thorough, beginner-friendly comments.

**Out of scope (unless explicitly requested):**
- Broad refactors across unrelated feature groups.
- Renaming triggers/behavior that chat depends on.
- Changing core stream-start/housekeeping behavior.
- Introducing unnecessary external dependencies.
