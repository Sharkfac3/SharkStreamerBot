# Conventions

## Git

### Direct to Main
Use for: small, focused, single-domain changes where scope is fully understood before starting.
- Change touches ≤ 2 files
- No risk of breaking live stream behavior
- Scope fully known before first edit
- Register in `WORKING.md` before starting

### Worktree Branch
Use for: multi-file, experimental, or parallel-agent work.
- Create from current `main`
- Branch naming: `<agent>/<descriptor>` (e.g., `claude/squad-cooldown-fix`, `pi/lotat-engine-v2`)
- Register branch in `WORKING.md` Active Work
- Follow Merge Review process in `Docs/AGENT-WORKFLOW.md` before merging

**Default is direct-to-main.** If you can describe the change in one sentence and it touches one file, go direct. If you need a paragraph, use a branch.

## File Naming

### Actions/
- Scripts: `<action>-main.cs`, `<action>-resolve.cs`, `<action>-call.cs`, etc.
- Folder depth: one level beneath feature group only — no deeper nesting
- Do not move scripts between feature groups unless explicitly requested

### Creative/
- Markdown docs use `SCREAMING-KEBAB-CASE.md` for canonical references (BRAND-IDENTITY, CHARACTER-CODEX)
- Agent files use lowercase kebab-case patterns like `character-name-art-agent.md` or `franchise-name-agent.md`

### .agents/
- Role folders: `kebab-case`
- Skill files: `_index.md` for folder overviews, `<topic>.md` for specifics
- Context files: any name, agents create freely

## Routing Rules

- `Actions/` — Streamer.bot runtime source only
- `Apps/` — TypeScript applications (overlay, broker, future web apps)
- `Tools/` — Mix It Up scripts, validators, sync helpers, Python utilities
- `Creative/` — art generation, worldbuilding, brand scaffolding
- `Docs/` — architecture, workflow, onboarding
- `.agents/` — agent skill/context tree (not runtime)
- `.pi/skills/` — Pi operational skill layer
- `.claude/` — Claude configuration

## Commit Style

- Imperative mood: "Add Pedro unlock wait", not "Added" or "Adding"
- Scope prefix when helpful: `ops:`, `squad:`, `brand:`, `lotat:`
- Include `synced-to-streamerbot: yes/no` in commit notes for `Actions/` changes
