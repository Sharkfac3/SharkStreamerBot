# Agent Workflow — Contributing to SharkStreamerBot

> For all contributors: human, pi, claude, diffusion, or otherwise.
>
> This document defines how work gets done in this repo — when to commit directly, when to use a branch, and how completed work gets reviewed and merged.

---

## The Two Contribution Modes

### Mode 1 — Direct to Main
**Use this for:** small, focused, single-domain changes where the scope is fully understood before starting.

Examples:
- Fixing a bug in one `.cs` file
- Updating a README or doc
- Adding a single helper snippet
- Tweaking brand voice copy in one file
- Minor lore corrections

**Rules:**
- Change touches ≤ 2 files
- No risk of breaking live stream behavior
- Scope is fully known before the first edit
- Register the task in `WORKING.md` before starting, mark complete when done

---

### Mode 2 — Worktree Branch
**Use this for:** anything multi-file, experimental, or where two or more agents might be working in parallel.

Examples:
- Adding a new Squad mini-game (touches script + README + SHARED-CONSTANTS + HELPER-SNIPPETS)
- Building a new story arc (touches story JSON + C# engine)
- Designing a new character (touches CHARACTER-CODEX + art agent + worldbuilding doc)
- Any change that might conflict with another active agent's work

**Rules:**
- Create a worktree branch from current `main`
- Name the branch using the conventions below
- Register the branch in `WORKING.md` under Active Work
- Work proceeds in the worktree; `main` stays clean until merge
- When complete, follow the Merge Review process below

---

## Branch Naming Conventions

```
<agent>/<descriptor>

claude/feature-name       — Claude-authored work
pi/feature-name           — Pi-authored work
diffusion/asset-name      — Art generation / diffusion workflow output
fix/short-description     — Bug fixes from any agent
ops/short-description     — Workflow, tooling, documentation changes
```

Descriptor should be lowercase, hyphen-separated, and specific enough to identify the work without reading the diff.

---

## Merge Review Process

Before merging a worktree branch to `main`, the branch author produces a **Merge Summary** using the template below. This is distinct from (but extends) the `change-summary` skill output — it adds the branch-level "why" context that individual file summaries don't capture.

The operator reviews the Merge Summary and confirms before the merge is executed.

### Merge Summary Template

```
## Merge Summary — <branch-name>

### Purpose
<One or two sentences: why did this branch exist? What problem does it solve?>

### Domain(s) Touched
- [ ] Actions/       (C# Streamer.bot scripts)
- [ ] Creative/      (Brand, art, lore, worldbuilding)
- [ ] Tools/         (Utilities, validators, sync helpers)
- [ ] Docs/          (Architecture, workflow, onboarding)
- [ ] .agents/       (Agent knowledge tree — roles, skills, context)
- [ ] .pi/skills/    (Pi operational skill layer)

### Files Changed
- `<path>` — <what changed>
- `<path>` — <what changed>

### Behavioral Summary
<What does the project do differently after this merge? Operator-friendly language.>

### Streamer.bot Paste Targets
| File | Action/Group |
|------|-------------|
| `<path>` | <Group > Action> |
*(Omit section if no Actions/ changes.)*

### Manual Setup Steps
<Any Streamer.bot UI actions required, or "None.">

### Validation Checklist
- [ ] Syntax check passed
- [ ] Happy path confirmed
- [ ] No SHARED-CONSTANTS regressions (see validation notes)
- [ ] Brand/canon reviewed if Creative/ touched
- [ ] WORKING.md updated (Active Work cleared, Recently Completed logged)
- [ ] No unresolved conflicts with other active branches

### Breaking Changes
<List any breaking changes, or "None.">

### Notes for Operator
<Anything the operator needs to know before or after merging — timing, stream state, dependencies.>
```

---

## WORKING.md Integration

The `WORKING.md` file and this workflow are tightly coupled:

| Workflow Step | WORKING.md Action |
|---------------|-------------------|
| Starting any task (either mode) | Add row to **Active Work** |
| Creating a worktree branch | Add branch name to the **Files Being Edited** column |
| Merge complete | Remove from **Active Work**, add to **Recently Completed** with commit hash |
| Identifying future work during a task | Flag to operator verbally — do not add to queue unilaterally |

---

## Conflict Resolution

If two agents have both modified the same file in separate branches:

1. The agent whose branch was created **later** is responsible for resolving conflicts
2. Conflicts in `Actions/` scripts → operator makes the final call (live stream reliability first)
3. Conflicts in `Creative/` brand/lore docs → `brand-canon-guardian` skill review required before resolving
4. Conflicts in `SHARED-CONSTANTS.md` → stop and flag to operator; do not auto-resolve

---

## The "Direct to Main" Default

Direct-to-main is the **default workflow** for this project. It reduces overhead for the common case (small focused changes) and keeps the repo history clean. Worktree branches exist for when parallel complexity makes direct-to-main risky — not as a requirement for all work.

When in doubt: if you can fully describe the change in one sentence and it touches one file, go direct. If you need a paragraph, use a branch.
