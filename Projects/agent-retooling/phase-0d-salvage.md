# Phase 0d — Salvage decisions

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` — confirm `phase-0a-audit.md` and `phase-0b-pi-format.md` are both checked off. If not, stop and wait.

## Mission

For every file in `audit.md`, decide whether it survives the retool. Output `Projects/agent-retooling/salvage-list.md`.

## Inputs

- `Projects/agent-retooling/audit.md` (from Phase 0a)
- `Projects/agent-retooling/pi-skill-format.md` (from Phase 0b)

## Verdicts

Tag each file as exactly one of:

- **keep** — survives the retool. Specify the proposed new path (best guess; Phase 4 finalizes).
- **merge** — content folds into an existing or planned canonical doc. Specify target.
- **kill** — gets deleted, content not worth preserving. Brief reason.

## Bias rules

**Bias toward kill** for:

- Role wrappers (`role.md` files)
- `routing-manifest.json`
- Stub redirects in `.pi/skills/`
- Sub-skill `_index.md` files that exist only for organization with no real content
- The `_template/` folder
- Per-role `context/` notes that are stale work logs or duplicate `_shared/` content

**Bias toward keep / merge** for:

- `_shared/` reference docs (project.md, conventions.md, coordination.md, mixitup-api.md, info-service-protocol.md)
- Atomic technique knowledge (helper snippets references, mini-game contribution contract)
- Workflow-shaped how-to content already written into a skill file
- Context notes that contain real reference-doc content (e.g. an undocumented API quirk) — tag **merge** with target `_shared/<topic>.md`

## Anti-drift reminder

PLAN.md decision: no `context/` subdirs in the new tree. Per-role context notes either merge into `_shared/` (if the content is reusable reference) or are killed (if stale or redundant). Do not propose any keep target that is a new `context/` folder.

## Output format — `salvage-list.md`

```markdown
# Salvage List — Verdicts

For every file in `audit.md`, exactly one verdict.

## keep

| current path | new path (proposed) | notes |
|---|---|---|
| .agents/_shared/project.md | .agents/_shared/project.md | preserved as canonical |
| ... | ... | ... |

## merge

| current path | merge target | notes |
|---|---|---|
| .agents/roles/streamerbot-dev/context/mixitup-quirks.md | .agents/_shared/mixitup-api.md | adds API quirk section |
| ... | ... | ... |

## kill

| current path | reason |
|---|---|
| .agents/roles/_template/role.md | role concept retired |
| ... | ... |

## Summary counts

- keep: <n>
- merge: <n>
- kill: <n>

## Open questions

- (files where verdict was unclear; describe the call you made and why)
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 0d complete: keep=<n>, merge=<n>, kill=<n>.`
2. Tick `[x] phase-0d-salvage.md` in STATUS.md.
3. Next: Phase 1 starts. Run `phase-1-generator.md` once `phase-0c-workflow-seeds.md` is also checked off.
