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
| `feature-squad` | Squad mini-games — Clone, Duck, Pedro, Toothless, offering. Subfeature map, shared constants, behavioral expectations. |
| `feature-commanders` | Commander role system — Captain Stretch, The Director, Water Wizard. 3-slot model, support commands, high score tracking. |
| `feature-twitch-integration` | Stream lifecycle — stream-start reset, Bits cheer forwarding, TTS wait behavior, idempotency rules. |
| `buildtools` | External Python/local tooling conventions for BuildTools/ utilities (MixItUp API, WorldBuilding). |
| `sync-workflow` | Repo-to-Streamer.bot paste process, validation checklists, commit note style. |
| `change-summary` | Standard response format for code changes — paste targets, setup steps, validation output. **Terminal skill — loaded after every code change.** |

## Routing

`AGENTS.md` contains the routing table that maps tasks to skills. Examples:

| Task | Skills loaded |
|---|---|
| Fix a Duck mini-game bug | `streamerbot-scripting` → `feature-squad` → `change-summary` |
| Add a new Commander command | `streamerbot-scripting` → `feature-commanders` → `change-summary` |
| Update stream-start reset | `streamerbot-scripting` → `feature-twitch-integration` → `change-summary` |
| Modify MixItUp API script | `buildtools` → `change-summary` |
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
